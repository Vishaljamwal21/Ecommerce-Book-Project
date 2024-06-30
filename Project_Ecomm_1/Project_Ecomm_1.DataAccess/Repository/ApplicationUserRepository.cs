﻿using Project_Ecomm_1.DataAccess.Data;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using Project_Ecomm_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_1.DataAccess.Repository
{
    public class ApplicationUserRepository:Repository<ApplicationUser>,IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;
        public ApplicationUserRepository (ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}
