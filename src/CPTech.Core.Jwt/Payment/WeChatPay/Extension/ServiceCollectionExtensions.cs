using CPTech.Payment.WeChatPay;
using Microsoft.Extensions.DependencyInjection;

namespace CPTech.Extension
{
    public static class WeChatPayExtension
    {
        public static void AddWeChatPay(this IServiceCollection services)
        {
            services.AddHttpClient(nameof(WeChatPayClient));
            services.AddSingleton<IWeChatPayClient, WeChatPayClient>();
        }
    }
}