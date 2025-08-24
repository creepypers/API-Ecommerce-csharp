using ECOM_UtilisateurMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ECOM_UtilisateurMicroservice.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UtilisateurDbContext _userDbContext;

        public UserController(UtilisateurDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        [HttpGet(Name = "GetUsers")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult GetUsers()
        {
            try
            {
                List<User> users = _userDbContext.Users.ToList();
                return Ok(users);
            }
            catch (Exception)
            {
                return BadRequest("Une erreur est survenue lors du traitement de la requête !");
            }
        }

        [HttpGet("{id}", Name = "GetUser")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult GetUser(int id)
        {
            try
            {
                User? user = _userDbContext.Users.Find(id);
                
                if (user == null)
                {
                    return NotFound($"L'utilisateur avec l'Id ({id}) n'existe pas !");
                }
                
                return Ok(user);
            }
            catch (Exception)
            {
                return BadRequest("Une erreur est survenue lors du traitement de la requête !");
            }
        }
        
        [HttpGet("by-email/{email}", Name = "GetUserByEmail")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult GetUserByEmail(string email)
        {
            try
            {
                var user = _userDbContext.Users.FirstOrDefault(u => u.Email == email);
                
                if (user == null)
                {
                    return NotFound($"L'utilisateur avec l'email ({email}) n'existe pas !");
                }
                
                return Ok(user);
            }
            catch (Exception)
            {
                return BadRequest("Une erreur est survenue lors du traitement de la requête !");
            }
        }

        [HttpPost(Name = "AddUser")]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public IActionResult AddUser([FromBody] User user)
        {
            try
            {
                if (user.UserType == UserType.Vendeur)
                {
                    if (string.IsNullOrWhiteSpace(user.CompanyName))
                    {
                        return BadRequest("Le nom de l'entreprise est obligatoire pour les comptes vendeur");
                    }
                }
                
                _userDbContext.Users.Add(user);
                _userDbContext.SaveChanges();
                return Created($"{Request.Host}{Request.PathBase}{Request.Path}{Request.QueryString}/{user.Id}", user);
            }
            catch (Exception)
            {
                return BadRequest("Une erreur est survenue lors du traitement de la requête !");
            }
        }

        [HttpPut("{id}", Name = "UpdateUser")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            try
            {
                var user = _userDbContext.Users.Find(id);
                if (user == null)
                {
                    return NotFound($"L'utilisateur avec l'Id ({id}) n'existe pas !");
                }

                if (updatedUser.UserType == UserType.Vendeur && string.IsNullOrWhiteSpace(updatedUser.CompanyName))
                {
                    return BadRequest("Le nom de l'entreprise est obligatoire pour les comptes vendeur");
                }

                user.Email = updatedUser.Email;
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.UserType = updatedUser.UserType;
                user.CompanyName = updatedUser.CompanyName;
                user.CompanyAddress = updatedUser.CompanyAddress;
                user.CompanyPhone = updatedUser.CompanyPhone;
                user.CompanyDescription = updatedUser.CompanyDescription;

                _userDbContext.SaveChanges();
                return Ok(user);
            }
            catch (Exception)
            {
                return BadRequest("Une erreur est survenue lors du traitement de la requête !");
            }
        }

        [HttpDelete("{id}", Name = "RemoveUser")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult RemoveUser(int id)
        {
            try
            {
                User? user = _userDbContext.Users.Find(id);
                if (user is not null)
                {
                    _userDbContext.Users.Remove(user);
                    _userDbContext.SaveChanges();
                    return Ok($"L'utilisateur avec l'Id ({id}) a été supprimé avec succès.");
                }
                else
                    return NotFound($"L'utilisateur avec l'Id ({id}) fourni n'existe pas !");
            }
            catch (Exception)
            {
                return BadRequest("Une erreur est survenue lors du traitement de la requête !");
            }
        }

        [HttpGet("clients", Name = "GetClients")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult GetClients()
        {
            try
            {
                List<User> clients = _userDbContext.Users
                    .Where(u => u.UserType == UserType.Client)
                    .ToList();
                return Ok(clients);
            }
            catch (Exception)
            {
                return BadRequest("Une erreur est survenue lors du traitement de la requête !");
            }
        }

        [HttpGet("vendeurs", Name = "GetVendeurs")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult GetVendeurs()
        {
            try
            {
                List<User> vendeurs = _userDbContext.Users
                    .Where(u => u.UserType == UserType.Vendeur)
                    .ToList();
                return Ok(vendeurs);
            }
            catch (Exception)
            {
                return BadRequest("Une erreur est survenue lors du traitement de la requête !");
            }
        }
    }
} 