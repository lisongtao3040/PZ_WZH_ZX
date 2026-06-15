using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace zyUpload.method
{
    /// <summary>
    /// imagesUp 的摘要说明
    /// </summary>
    public class imagesUp : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            //上传配置
            int size = 2;           //文件大小限制,单位MB                             //文件大小限制，单位MB
            string[] filetype = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };         //文件允许格式


            //上传图片
            Hashtable info = new Hashtable();
            zyUpload.method.Uploader up = new zyUpload.method.Uploader();
            //string path = Constant.IMAGES_UP_PATH;
            string path = "/upload/images";
            info = up.upFile(context, path + '/', filetype, size);                   //获取上传状态
            HttpContext.Current.Response.Write(info["url"]);  //向浏览器返回数据json数据
            
            //D:\GITEE\wzh-and-zx\wzh\zyUpload\method
            //D:\GITEE\wzh-and-zx\wzh\zyUpload\upload\images
            string dir = HttpContext.Current.Request.PhysicalApplicationPath;

            //D:\GITEE\wzh - and - zx\wzh\
            DirectoryInfo pathInfo = new DirectoryInfo(dir);
            string parentDir = pathInfo.Parent.FullName;

            string imgDir = dir + @"upload\images\";
            string realImgDir = parentDir + @"\web\Image\ChkImgs\";

            try
            {
                System.IO.File.Copy(imgDir + info["originalName"], realImgDir + info["originalName"], true);
                System.IO.File.Delete(imgDir + info["originalName"]);
            }
            catch
            {

            }
   

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}