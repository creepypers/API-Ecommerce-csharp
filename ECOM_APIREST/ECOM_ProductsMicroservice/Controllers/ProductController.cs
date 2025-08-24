using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using ECOM_ProductsMicroservice;
using ECOM_ProductsMicroservice.Models;

namespace ECOM_ProductsMicroservice.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductDbContext _productDbContext;

        public ProductController(ProductDbContext productDbContext)
        {
            _productDbContext = productDbContext;
        }

        [HttpGet(Name = "GetProducts")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult GetProducts()
        {
            try
            {
                List<Product> products = _productDbContext.Products.ToList();
                return Ok(products);
            }
            catch (Exception)
            {
                return BadRequest("Une erreur est survenue lors du traitement de la requête !");
            }
        }

        [HttpGet("{productId}", Name = "GetProductById")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult GetProductById(int productId)
        {
            try
            {
                Product? product = _productDbContext.Products
                    .FirstOrDefault(p => p.ProductId == productId);

                if (product is not null)
                {
                    return Ok(product);
                }
                else
                {
                    return NotFound($"Le produit avec l'Id ({productId}) n'existe pas !");
                }
            }
            catch (Exception)
            {
                return BadRequest("Une erreur est survenue lors du traitement de la requête !");
            }
        }

        [HttpPost(Name = "AddProduct")]
        [Authorize(Roles = "Vendeur")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public IActionResult AddProduct([FromBody] ProductModel model)
        {
            try
            {
                var sellerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(sellerId))
                {
                    return Unauthorized("L'identifiant du vendeur n'a pas été trouvé dans le token");
                }
                
                int sellerIdInt = int.Parse(sellerId);
                
                Product product = new Product() 
                { 
                    Name = model.Name,
                    ShortDescription = model.ShortDescription,
                    Description = model.Description,
                    Price = model.Price,
                    Category = model.Category,
                    ImageUrl = model.ImageUrl,
                    IsNewArrival = model.IsNewArrival,
                    CreatedAt = DateTime.Now,
                    Rating = model.Rating,
                    SellerId = sellerIdInt  
                };

                if (product.SellerId != sellerIdInt)
                {
                    return Forbid("Vous ne pouvez pas ajouter des produits pour d'autres vendeurs");
                }

                _productDbContext.Products.Add(product);
                _productDbContext.SaveChanges();

                return CreatedAtAction(nameof(GetProductById), new { productId = product.ProductId }, product);
            }
            catch (Exception ex) 
            {
                return BadRequest($"Une erreur est survenue lors du traitement de la requête : {ex.Message}");
            }
        }

        [HttpDelete("{productId}", Name = "RemoveProduct")]
        [Authorize(Roles = "Vendeur")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public IActionResult RemoveProduct(int productId)
        {
            try
            {
                var sellerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(sellerId))
                {
                    return Unauthorized("L'identifiant du vendeur n'a pas été trouvé dans le token");
                }
                
                int sellerIdInt = int.Parse(sellerId);

                Product? product = _productDbContext.Products.Find(productId);
                if (product is not null)
                {
                    if (sellerIdInt != product.SellerId)
                    {
                        return Forbid("Vous ne pouvez pas supprimer les produits d'autres vendeurs");
                    }
                    
                    _productDbContext.Products.Remove(product);
                    _productDbContext.SaveChanges();
                    return Ok($"Le produit avec l'Id ({productId}) a été supprimé avec succès.");
                }
                else
                    return NotFound($"Le produit avec l'Id ({productId}) n'existe pas !");
            }
            catch (Exception ex)
            {
                return BadRequest($"Une erreur est survenue lors du traitement de la requête : {ex.Message}");
            }
        }

        [HttpGet("category/{category}", Name = "GetProductsByCategory")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult GetProductsByCategory(string category)
        {
            try
            {
                List<Product> products = _productDbContext.Products
                    .Where(p => p.Category == category)
                    .ToList();
                
                return Ok(products);
            }
            catch (Exception)
            {
                return BadRequest("Une erreur est survenue lors du traitement de la requête !");
            }
        }

        [HttpGet("new-arrivals", Name = "GetNewArrivals")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult GetNewArrivals()
        {
            try
            {
                List<Product> products = _productDbContext.Products
                    .Where(p => p.IsNewArrival)
                    .ToList();
                
                return Ok(products);
            }
            catch (Exception)
            {
                return BadRequest("Une erreur est survenue lors du traitement de la requête !");
            }
        }

        [HttpGet("search", Name = "SearchProducts")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult SearchProducts([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest("Le terme de recherche ne peut pas être vide.");
                }

                List<Product> products = _productDbContext.Products
                    .Where(p => p.Name.Contains(query) || 
                                p.Description.Contains(query) || 
                                p.ShortDescription.Contains(query))
                    .ToList();
                
                return Ok(products);
            }
            catch (Exception)
            {
                return BadRequest("Une erreur est survenue lors du traitement de la requête !");
            }
        }

        [HttpPut("{productId}", Name = "UpdateProduct")]
        [Authorize(Roles = "Vendeur")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public IActionResult UpdateProduct(int productId, [FromBody] ProductModel model)
        {
            try
            {
                var sellerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(sellerId))
                {
                    return Unauthorized("L'identifiant du vendeur n'a pas été trouvé dans le token");
                }
                
                int sellerIdInt = int.Parse(sellerId);
                
                Product? product = _productDbContext.Products.Find(productId);
                
                if (product is null)
                {
                    return NotFound($"Le produit avec l'Id ({productId}) n'existe pas !");
                }
                
                if (sellerIdInt != product.SellerId)
                {
                    return Forbid("Vous ne pouvez pas modifier les produits d'autres vendeurs");
                }

                product.Name = model.Name;
                product.ShortDescription = model.ShortDescription;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Category = model.Category;
                product.ImageUrl = model.ImageUrl;
                product.IsNewArrival = model.IsNewArrival;
                product.Rating = model.Rating;

                _productDbContext.SaveChanges();
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest($"Une erreur est survenue lors du traitement de la requête : {ex.Message}");
            }
        }
    }
}
