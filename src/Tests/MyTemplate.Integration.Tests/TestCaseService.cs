using System;
using System.Linq;
using Aertssen.Framework.Data.Extensions;
using Aertssen.Framework.Tests;
using MyTemplate.Data;
using MyTemplate.Domain.Model;
using MyTemplate.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace MyTemplate.Integration.Tests
{
    public class TestCaseService
    {
        private readonly MyTemplateDbContext _context;
        private readonly TestCaseFactory _factory;

        public TestCaseService(MyTemplateDbContext dbContext)
        {
            _context = dbContext;
            _factory = new TestCaseFactory();
        }
        
    }


}
