using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRM.Models;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;

namespace HRM.Controllers


{
    public class HighlightKataloguesController : Controller
    {
        private HRMEntities db = new HRMEntities();
        private string sSql = "";
        private string locimgpath = "~/image/CustAppImages/";
        private string imgtemppath = "~/image/CustAppImages/ImagesTemps";


        public ActionResult Index()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");
            return View(db.HighlightKatalogues.ToList());
        }

        [HttpPost]
        
        public ActionResult GetDataList()
        {
            var msg = "";

            sSql = "SELECT ID, ID [NO], TITLE [TITLE], SUBTITLE [SUBTITLE] FROM HighlightKatalogues";
            if (msg == "")
            {
                DataTable tbl = new ClassConnection().GetDataTable(sSql, "HighlightKatalogues");

                if (tbl.Rows.Count > 0)
                {
                    List<string> colname = new List<string>();

                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in tbl.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in tbl.Columns)
                        {
                            var item = dr[col].ToString();
                            if (col.ColumnName == "NO")
                            {
                                item = "<a class='text-primary' href='" + Url.Action("Form/" + dr["ID"].ToString(), "HighlightKatalogues") + "'>" + item + "</a>";
                            }
                            row.Add(col.ColumnName, item);
                            if (!colname.Contains(col.ColumnName))
                                colname.Add(col.ColumnName);
                        }
                        rows.Add(row);
                    }

                    JsonResult js = Json(new { msg, colname, rows }, JsonRequestBehavior.AllowGet);
                    js.MaxJsonLength = Int32.MaxValue;
                    return js;
                }
                else
                    msg = "Data Not Found";
            }

            return Json(new { msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HighlightKatalogues qL_mstcurr = db.HighlightKatalogues.Find(id);
            if (qL_mstcurr == null)
            {
                return HttpNotFound();
            }
            return View(qL_mstcurr);
        }

        public ActionResult Form(int? id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");

            HighlightKatalogues HighlightKatalogues;
            string action = "New Data";
            if (id == null)
            {

                HighlightKatalogues = new HighlightKatalogues();
                HighlightKatalogues.CREATED_AT = ClassFunction.GetServerTime();

            }
            else
            {
                action = "Update Data";
                HighlightKatalogues = db.HighlightKatalogues.Find(id);

              
            }

            if (HighlightKatalogues == null)
            {
                return HttpNotFound();
            }

           
            ViewBag.action = action;
            return View(HighlightKatalogues);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Form(HighlightKatalogues HighlightKatalogues, string action)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");
            HighlightKatalogues.CREATED_AT = ClassFunction.GetServerTime();
            sSql = "select top 1 max(ID) + 1 from HighlightKatalogues";
            var mstoid = db.Database.SqlQuery<Decimal>(sSql).FirstOrDefault();
            if (ModelState.IsValid)
            {
                using (var objTrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (System.IO.File.Exists(Server.MapPath(HighlightKatalogues.IMAGE_COVER)))
                        {
                            var sExt = Path.GetExtension(HighlightKatalogues.IMAGE_COVER);
                            var sdir = Server.MapPath(locimgpath);
                            var sfilename = HighlightKatalogues.ID + sExt;
                            if (!Directory.Exists(sdir))
                            {
                                DirectorySecurity securityRules = new DirectorySecurity();
                                securityRules.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                                Directory.CreateDirectory(sdir, securityRules);
                            }
                            if (HighlightKatalogues.IMAGE_COVER.ToLower() != (locimgpath + "/" + sfilename).ToLower())
                            {
                                System.IO.File.Delete(sdir + "/" + sfilename);
                                System.IO.File.Move(Server.MapPath(HighlightKatalogues.IMAGE_COVER), sdir + "/" + sfilename);
                            }
                            HighlightKatalogues.IMAGE_COVER = locimgpath + "/" + sfilename;
                        }

                        if (action == "New Data")
                        {
                            sSql = "INSERT INTO HighlightKatalogues (TITLE, SUBTITLE,IMAGE_COVER, CREATED_AT, UPDATED_AT) VALUES('" + HighlightKatalogues.TITLE + "', '" + HighlightKatalogues.SUBTITLE + "', '" + Request.Url.Scheme + "://" + Request.Url.Host + ":90" + System.Web.VirtualPathUtility.ToAbsolute(HighlightKatalogues.IMAGE_COVER) + "', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";
                            db.Database.ExecuteSqlCommand(sSql);
                            db.SaveChanges();
                        }
                        else
                        {
                            sSql = "UPDATE HighlightKatalogues SET TITLE='" + HighlightKatalogues.TITLE + "', SUBTITLE='" + HighlightKatalogues.SUBTITLE + "', IMAGE_COVER='" + Request.Url.Scheme + "://" + Request.Url.Host + ":90" + System.Web.VirtualPathUtility.ToAbsolute(HighlightKatalogues.IMAGE_COVER) + "' , CREATED_AT=CURRENT_TIMESTAMP, UPDATED_AT=CURRENT_TIMESTAMP WHERE ID=" + HighlightKatalogues.ID + "";
                            db.Database.ExecuteSqlCommand(sSql);

                            db.SaveChanges();
                        }

                        objTrans.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        objTrans.Rollback();
                        ModelState.AddModelError("", ex.ToString());
                    }
                }
            }
            ViewBag.action = action;
            return View(HighlightKatalogues);
        }
        public class oidusage
        {
            public string tblusage { get; set; }
            [AllowHtml]
            public string colusage { get; set; }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");
            HighlightKatalogues list = db.HighlightKatalogues.Find(id);
            var servertime = ClassFunction.GetServerTime();

            string result = "success";
            string msg = "";
            if (list == null)
            {
                result = "failed";
                msg = "Data can't be found!";
            }

            if (result == "success")
            {
                using (var objTrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (list != null)
                        {
                            if (System.IO.File.Exists(Server.MapPath(list.IMAGE_COVER)))
                            {
                                System.IO.File.Delete(Server.MapPath(list.IMAGE_COVER));
                            }

                        }
                        sSql = "DELETE FROM HighlightKatalogues WHERE ID=" + list.ID;
                        db.Database.ExecuteSqlCommand(sSql);
                        db.SaveChanges();

                        objTrans.Commit();
                    }
                    catch (Exception ex)
                    {
                        objTrans.Rollback();
                        result = "failed";
                        msg = ex.ToString();
                    }
                }
            }
            return Json(new { result, msg }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> UploadFile()
        {
            var result = "";
            var idimgpath = "";
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file] as HttpPostedFileBase;
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        //var fileName = Path.GetFileName(fileContent.FileName);
                        var sfilename = Path.GetRandomFileName().Replace(".", "");
                        var sext = Path.GetExtension(fileContent.FileName);
                        var sdir = Server.MapPath(imgtemppath);
                        var path = Path.Combine(sdir, sfilename + sext);

                        idimgpath = imgtemppath + "/" + sfilename + sext;
                        if (!Directory.Exists(sdir))
                        {
                            DirectorySecurity securityRules = new DirectorySecurity();
                            securityRules.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                            Directory.CreateDirectory(sdir, securityRules);
                        }
                        using (var stream2 = new FileStream(path, FileMode.Create))
                        {
                            await stream.CopyToAsync(stream2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                result = "Upload failed" + ex.Message;
                return Json(result, idimgpath);
            }
            result = "sukses";

            return Json(new { result, idimgpath }, JsonRequestBehavior.AllowGet);
        }
        public bool IsInputValid(HighlightKatalogues HighlightKatalogues)
        {
            bool isValid = true;


            if (!ModelState.IsValid)
            {
                isValid = false;

            }
            return isValid;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
