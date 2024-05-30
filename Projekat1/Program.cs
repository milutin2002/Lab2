using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Projekat1.Models;
using Projekat1.Server;
using Projekat1.Services;

namespace Projekat1
{
    internal class Program
    {
        
        public static async Task Main(string[] args)
        {
            HttpServer server = new HttpServer("http://localhost:5050/");
            await server.start();
        }
    }
}