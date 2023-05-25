using JsonDeserializator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JsonDeserializator.Controllers
{
    public class DeserializerController : Controller
    {
        // GET: DeserializerController
        /*Контроллер отвечает за возможность загрузки json файла в формате динамического объекта, который десериализуется на сервере и представляется в виде таблицы, соответственно,
         * в контроллер должен падать динамический объект, определяемый в коде (представление типа drag-n-drop), после чего происходит обработка в контроллере, буферизация файла.
         * Index — возвращает окно для загрузки файлов, а также показывает список уже записанных файлов на сервер. 
         * public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)  сохраняет файлы на сервере.
         * Размер файла не более 50 мегабайт. 
         * 
         * 
         */
        //Открывает окно для загрузки JSON файла для его десериализации, отправляет список уже загруженных ранее файлов. 
        public ActionResult Index()
        {
            SingleFileModel model = new SingleFileModel();
            var path = Environment.CurrentDirectory + @"\UploadedJsonFiles";
            DirectoryInfo dir = new DirectoryInfo(path);

            if (!dir.Exists)
            {
             dir.Create();
            }
            List<string> list = new List<string>();
            string[] files = Directory.GetFiles(path);
            foreach (string s in files)
            {
                list.Add(s);
            }
            return View(list);
        }

        public IActionResult Upload()
        {
            SingleFileModel model = new SingleFileModel();
            return View(model);
        }

        public IActionResult MultiFile()
        {
            MultipleFilesModel model = new MultipleFilesModel();
            return View(model);
        }

        //Сохраняет файл, пока что скрыто, т.к. есть проблема передачи файла.
        [HttpPost]
        public IActionResult UploadSave(SingleFileModel model)
        {
            if (ModelState.IsValid)
            {
                model.IsResponse = true;

                var path = Environment.CurrentDirectory + @"\UploadedJsonFiles";

                //create folder if not exist
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //get file extension
                FileInfo fileInfo = new FileInfo(model.File.FileName);
                string fileName = model.FileName + fileInfo.Extension;

                string fileNameWithPath = Path.Combine(path, fileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    model.File.CopyTo(stream);
                }
                model.IsSuccess = true;
                model.Message = "File upload successfully";
            }
            return View("Index", model);
        }

        //Мульти-загрузка файлов, сейчас работает и загружает файлы в директорию, но необходимо написать проверку типа isExist
        [HttpPost]
        public IActionResult MultiUpload(MultipleFilesModel model)
        {
            //if (ModelState.IsValid)
            //{
                model.IsResponse = true;
                if (model.Files.Count > 0)
                {
                    foreach (var file in model.Files)
                    {

                        var path = Environment.CurrentDirectory + @"\UploadedJsonFiles";

                        //create folder if not exist
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);


                        string fileNameWithPath = Path.Combine(path, file.FileName);

                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                    }
                    model.IsSuccess = true;
                    model.Message = "Files upload successfully";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Message = "Please select files";
                }
            //}
            return View("MultiFile", model);
        }
        ////Метод-заглушка для загрузки
        //public async Task<IActionResult> Upload(IFormFile file)
        //{
        //    string filePath = Environment.CurrentDirectory + @"\UploadedJsonFiles";
        //    string block = "Этот метод работает";
        //    return View(block);
        //}


        //Контроллер, выводящий список файлов по их сокращённому названию и дающий метод вывода данных в таблицу по id
        public IActionResult JSONDeserialize()
        {
            return View();
        }
        //Post-метод index проводит RedirrectToAction(GetTableView) в метод записи файла + проводит валидацию на то, есть ли такой файл уже в системе

        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.GetTempFileName();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size });
        }

        // GET: DeserializerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DeserializerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DeserializerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DeserializerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DeserializerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DeserializerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DeserializerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
