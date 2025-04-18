using System.Xml.Linq;
using System.Xml.Schema;
using GAC.WMS.Integration.Application.Interfaces;
using GAC.WMS.Integration.Application.Models.Requests;

namespace GAC.WMS.Integration.FileProcessor
{
    public class FileProcessorService : BackgroundService
    {
        private readonly ILogger<FileProcessorService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly FileSystemWatcher _fileWatcher;

        public FileProcessorService(
            ILogger<FileProcessorService> logger,
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;

            var watchPath = _configuration["FileProcessor:WatchPath"];
            var filter = _configuration["FileProcessor:FileFilter"] ?? "*.xml";

            _fileWatcher = new FileSystemWatcher(watchPath, filter)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("File Processor Service running.");

            _fileWatcher.Created += OnFileCreated;
            _fileWatcher.EnableRaisingEvents = true;

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _fileWatcher.EnableRaisingEvents = false;
            _fileWatcher.Dispose();
        }

        private async void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"New file detected: {e.FullPath}");

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                    var purchaseOrderService = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
                    var salesOrderService = scope.ServiceProvider.GetRequiredService<ISalesOrderService>();

                    await ProcessFileAsync(e.FullPath, customerService, productService, purchaseOrderService, salesOrderService);
                }
                _logger.LogInformation($"File processed successfully: {e.FullPath}");
            }
            catch (XmlSchemaValidationException ex)
            {
                _logger.LogError(ex, $"Schema validation failed for file: {e.FullPath}");
                MoveToErrorFolder(e.FullPath, "ValidationErrors");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing file: {e.FullPath}");
                // Move to error folder
                var errorPath = Path.Combine(_configuration["FileProcessor:ErrorPath"], Path.GetFileName(e.FullPath));
                File.Move(e.FullPath, errorPath);
            }
        }

        private async Task ProcessFileAsync(string filePath,
            ICustomerService customerService,
            IProductService productService,
            IPurchaseOrderService purchaseOrderService,
            ISalesOrderService salesOrderService)
        {
            var xmlDoc = XDocument.Load(filePath);
            if(xmlDoc is null)
            {
                throw new NullReferenceException($"Failed to load XML document from path: {filePath}.");
            }
            var root = xmlDoc.Root;
            
            //To get base schema xsd path to validate the xml file.
            var baseSchemaPath = _configuration["FileProcessor:SchemaPath"];
            
            // Validate against schema first -- 
            // Note this code is commented noe, use when actual xsd has been placed under the schema folder

            //var schemaPath = SchemaValidator.GetSchemaPath(root.Name.LocalName, baseSchemaPath!);
            //SchemaValidator.ValidateXml(filePath, schemaPath);

            switch (root!.Name.LocalName)
            {
                case "Customer":
                    await ProcessCustomerFile(root, customerService);
                    break;
                case "Product":
                    await ProcessProductFile(root, productService);
                    break;
                case "PurchaseOrder":
                    await ProcessPurchaseOrderFile(root, customerService, productService, purchaseOrderService);
                    break;
                case "SalesOrder":
                    await ProcessSalesOrderFile(root, customerService, productService, salesOrderService);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown file type: {root.Name.LocalName}");
            }

            // Move to processed folder
            var processedPath = Path.Combine(_configuration["FileProcessor:ProcessedPath"], Path.GetFileName(filePath));
            File.Move(filePath, processedPath);
        }

        private async Task ProcessCustomerFile(XElement root, ICustomerService customerService)
        {
            var customer = new CustomerDto
            {
                CustomerIdentifier = root.Element("CustomerIdentifier")?.Value,
                Name = root.Element("Name")?.Value,
                Address = root.Element("Address")?.Value
            };

            await customerService.CreateCustomerAsync(customer);
        }

        private async Task ProcessProductFile(XElement root, IProductService productService)
        {
            var product = new ProductDto
            {
                ProductCode = root.Element("Code")?.Value,
                Title = root.Element("Title")?.Value,
                Description = root.Element("Description")?.Value,
                Length = decimal.Parse(root.Element("Length")?.Value ?? "0"),
                Width = decimal.Parse(root.Element("Width")?.Value ?? "0"),
                Height = decimal.Parse(root.Element("Height")?.Value ?? "0"),
                Weight = decimal.Parse(root.Element("Weight")?.Value ?? "0")
            };

            await productService.CreateProductAsync(product);
        }

        private async Task ProcessPurchaseOrderFile(XElement root,
            ICustomerService customerService,
            IProductService productService,
            IPurchaseOrderService purchaseOrderService)
        {
            var customerId = int.Parse(root.Element("CustomerId")?.Value ?? "0");
            var customer = await customerService.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with ID {customerId} not found");
            }

            var order = new PurchaseOrderDto
            {
                OrderId = root.Element("OrderId")?.Value,
                ProcessingDate = DateTime.Parse(root.Element("ProcessingDate")?.Value ?? DateTime.UtcNow.ToString()),
                CustomerId = customerId
            };

            foreach (var itemElement in root.Element("Items").Elements("Item"))
            {
                var productCode = itemElement.Element("ProductCode")?.Value;
                var product = await productService.GetProductByCodeAsync(productCode);
                if (product == null)
                {
                    throw new ArgumentException($"Product with code {productCode} not found");
                }

                order.Items.Add(new PurchaseOrderItemDto
                {
                    ProductCode = productCode,
                    Quantity = int.Parse(itemElement.Element("Quantity")?.Value ?? "0"),
                });
            }

            await purchaseOrderService.CreatePurchaseOrderAsync(order);
        }

        private async Task ProcessSalesOrderFile(XElement root,
            ICustomerService customerService,
            IProductService productService,
            ISalesOrderService salesOrderService)
        {
            var customerId = int.Parse(root.Element("CustomerId")?.Value ?? "0");
            var customer = await customerService.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with ID {customerId} not found");
            }

            var order = new SalesOrderDto
            {
                OrderId = root.Element("OrderId")?.Value,
                ProcessingDate = DateTime.Parse(root.Element("ProcessingDate")?.Value ?? DateTime.UtcNow.ToString()),
                CustomerId = customerId,
                ShipmentAddress = root.Element("ShipmentAddress")?.Value
            };

            foreach (var itemElement in root.Element("Items").Elements("Item"))
            {
                var productCode = itemElement.Element("ProductCode")?.Value;
                var product = await productService.GetProductByCodeAsync(productCode);
                if (product == null)
                {
                    throw new ArgumentException($"Product with code {productCode} not found");
                }

                order.Items.Add(new SalesOrderItemDto
                {
                    ProductCode = productCode,
                    Quantity = int.Parse(itemElement.Element("Quantity")?.Value ?? "0"),
                });
            }

            await salesOrderService.CreateSalesOrderAsync(order);
        }
        private void MoveToErrorFolder(string filePath, string errorType)
        {
            var errorDir = Path.Combine(_configuration["FileProcessor:ErrorPath"], errorType);
            Directory.CreateDirectory(errorDir);
            var errorPath = Path.Combine(errorDir, Path.GetFileName(filePath));

            try
            {
                if (File.Exists(errorPath))
                    File.Delete(errorPath);
                File.Move(filePath, errorPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to move file {filePath} to error directory");
            }
        }
    }
}