using JsonDeserializator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;


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
            FileModel model = new FileModel();
            var path = Environment.CurrentDirectory + @"\UploadedJsonFiles";
            DirectoryInfo dir = new DirectoryInfo(path);

            if (!dir.Exists)
            {
             dir.Create();
            }
            //инициализация модели файла
            List<FileModel> list = new List<FileModel>();
            //перебор всех файлов, добавление id для файла
            int i = 0;
            foreach (FileInfo file in dir.GetFiles())
            {
                i++;
                list.Add(new FileModel() { Id = i, Name = file.Name, Path = file.DirectoryName });
            }
            //массив с 
            //string[] files = Directory.GetFiles(path);
           
            //foreach (var s in files.Select((value,i) => new { i, value }))
            //{
            //    new FileModel()
            //}
            ViewBag.model = list;
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


        //Контроллер, открывающий файл в новом окне с рутом по id
        public async Task<IActionResult> ReadFile(int id)
        {
            if(id == null || id < 0)
            {
                return View("Error");
            }
            //иниц. id
            
            FileModel model = new FileModel();
            var path = Environment.CurrentDirectory + @"\UploadedJsonFiles";
            DirectoryInfo dir = new DirectoryInfo(path);
            List<FileInfo> files = new List<FileInfo>();
            //Заполняем коллекцию файлами
            foreach(FileInfo f in dir.GetFiles())
            {
                files.Add(f);
            }
            //Возвращаем нужный файл
            FileInfo file = files[id];
            path = Convert.ToString(file.Name);
            //using(StreamReader content = new StreamReader(file.DirectoryName))
            //{
            //    string? line;
            //    while ((line = await content.ReadLineAsync()) != null)
            //    {

            //    }
            //}

            //List<string> lines = new List<string>();
            //using (StreamReader content = new StreamReader(Convert.ToString(file)))
            //{
            //    string? line;
            //    while ((line = content.ReadLine()) != null)
            //    {
            //        lines.Add(line);
            //    }
            //}
            return View(file);
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
