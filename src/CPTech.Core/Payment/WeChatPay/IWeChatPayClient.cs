using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CPTech.Payment.WeChatPay
{
    public interface IWeChatPayClient
    {
        Task<string> GetAsync(string url);

        Task<JsonDocument> ExecuteAsync(IWeChatLoginRequest request, WeChatPayOptions options);

        /// <summary>
        /// 执行 WeChatPay API请求。
        /// </summary>
        /// <param name="request">具体的WeChatPay API请求</param>
        /// <param name="options">配置选项</param>
        /// <returns>领域对象</returns>
        Task<T> ExecuteAsync<T>(IWeChatPayRequest<T> request, WeChatPayOptions options) where T : WeChatPayResponse;

        /// <summary>
        /// 执行 WeChatPay API请求。
        /// </summary>
        /// <param name="request">具体的WeChatPay API请求</param>
        /// <param name="options">配置选项</param>
        /// <returns>领域对象</returns>
        Task<T> PageExecuteAsync<T>(IWeChatPayRequest<T> request, WeChatPayOptions options) where T : WeChatPayResponse;

        /// <summary>
        /// 执行 WeChatPay Sdk请求。
        /// </summary>
        /// <param name="request">具体的WeChatPay Sdk请求</param>
        /// <param name="options">配置选项</param>
        Task<WeChatPayDictionary> ExecuteAsync(IWeChatPayLiteAppSdkRequest request, WeChatPayOptions options);

        /// <summary>
        /// 执行 WeChatPay 通知请求解析
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">控制器的请求</param>
        /// <param name="options">配置选项</param>
        /// <returns>领域对象</returns>
        Task<T> ExecuteAsync<T>(HttpRequest request, WeChatPayOptions options) where T : WeChatPayNotify;
    }
}
