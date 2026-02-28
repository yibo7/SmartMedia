using System.Diagnostics;

namespace SmartMedia.MCore
{
    public static class XsComm
    {
         

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="source">要下载的文件地址</param>
        /// <param name="target">要保存的完整路径</param>
        /// <param name="progress">回调的进度，就是0-100</param>
        /// <returns>返回空值表示下载成功，返回非空表示有错误信息</returns>
        static public async Task<string> DownFileAsync(string source, string target, Action<double> progress)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(source);
                    response.EnsureSuccessStatusCode();

                    long totalBytes = response.Content.Headers.ContentLength.GetValueOrDefault();
                    long downloadedBytes = 0;

                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    using (FileStream streamToWriteTo = new FileStream(target, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length)) != 0)
                        {
                            await streamToWriteTo.WriteAsync(buffer, 0, bytesRead);
                            downloadedBytes += bytesRead;

                            // 假设 downloadedBytes 和 totalBytes 已经定义
                            double progressPercentage = (double)downloadedBytes / totalBytes * 100;

                            // 四舍五入到小数点后两位
                            progressPercentage = Math.Round(progressPercentage, 2);

                            progress(progressPercentage);
                            //progress?.Report((double)downloadedBytes / totalBytes * 100); // Report progress

                        }
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


    }
}
