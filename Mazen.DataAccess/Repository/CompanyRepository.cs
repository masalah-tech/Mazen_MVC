﻿using MazenWebApp.DataAccess.Data;
using MazenWebApp.DataAccess.Repository.IRepository;
using MazenWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazenWebApp.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public void Update(Company company)
        {
            _context.Companies.Update(company);
        }
    }
}
