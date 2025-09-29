using Microsoft.AspNetCore.Mvc;
using NeolantTestTask.Models;
using NeolantTestTask.Repositories;
using System.Threading.Tasks;

namespace NeolantTestTask.Controllers
{
    public class DataController : Controller
    {
        private readonly IDataSourceRepository _dataSourceRepository;

        public DataController(IDataSourceRepository dataSourceRepository)
        {
            _dataSourceRepository = dataSourceRepository;
        }

        public async Task<IActionResult> Index()
        {
            var sources = await _dataSourceRepository.GetAllAsync();
            return View(sources);
        }


        public async Task<IActionResult> Panel()
        {
            var sources = await _dataSourceRepository.GetAllAsync();
            return PartialView("_Panel", sources);
        }


        public async Task<IActionResult> Partial1()
        {
            var sources = await _dataSourceRepository.GetAllAsync();
            return PartialView("_Partial1", sources);
        }


        public async Task<IActionResult> Partial2()
        {
            var sources = await _dataSourceRepository.GetAllAsync();
            return PartialView("_Partial2", sources);
        }


        public async Task<IActionResult> Partial3()
        {
            var sources = await _dataSourceRepository.GetAllAsync();
            return PartialView("_Partial3", sources);
        }
    }
}