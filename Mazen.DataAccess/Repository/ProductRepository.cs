using Mazen.DataAccess.Repository.IRepository;
using MazenWebApp.Models;
using MazenWebApp.DataAccess.Data;
using MazenWebApp.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Mazen.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            //_context.Entry(product).State = EntityState.Detached;
            _context.Products.Update(product);
        }
    }
}
