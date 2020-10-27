using CPTech.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Encrypt
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly string APIGW_PUBLIC_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA7joSdXvKEPn9rl8DjbDOIx0A4qv4rKKguCpfS6j265NJMHwm/2QB2eY9Zbw2XIosi+9TVrp601/2KvlaG4T3IEl64icX7eG2mZ3iUfec/kTnjziRk7jMUxeMpJkGMnWfgxcTu7XPIIJEFiYaXcrGSar0BpM4Ys4k9yq5BbmdWfPZZpRvbuTWXSjJkIhVFIYgiy/dDet1cYBJs71xUwjzVQjFFPdpaNtI6uehMf83Qy4KwYJY75tqpVUPqmw5Levq+yFDT81A1+bG0LBZ1LH8GQrZR+IYY70IBxWKGrYG3xLrWPpU/YTJK/WhEeTOf0Yp38aLdqvKeC11hv6Pghhu5QIDAQAB";

        private static readonly string MY_PRIVATE_KEY = "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDuOhJ1e8oQ+f2uXwONsM4jHQDiq/isoqC4Kl9LqPbrk0kwfCb/ZAHZ5j1lvDZciiyL71NWunrTX/Yq+VobhPcgSXriJxft4baZneJR95z+ROePOJGTuMxTF4ykmQYydZ+DFxO7tc8ggkQWJhpdysZJqvQGkzhiziT3KrkFuZ1Z89lmlG9u5NZdKMmQiFUUhiCLL90N63VxgEmzvXFTCPNVCMUU92lo20jq56Ex/zdDLgrBgljvm2qlVQ+qbDkt6+r7IUNPzUDX5sbQsFnUsfwZCtlH4hhjvQgHFYoatgbfEutY+lT9hMkr9aER5M5/Rinfxot2q8p4LXWG/o+CGG7lAgMBAAECggEASec0EMQ9VHTpUO3i4VLnMBdwTeGBvLSa3Wcvtv4M09oWS3dIddQlit6VT3lK9/xZg3PUS8SAFBDiAkTmwkDGlXqWJ5BvKPnrts43BqylRHBF0bV/2+7pXCGjHPDn7kF1IarIEWR+hBxFWQWrCku8pd/l4iBLzsMazp6vVWpWuS+Gi10C5oJjZ2TBYyxlkQAxeAle+D2NScIujL0lm1qCgprybs0TG2uzTRiqUZaUAdTbBmVyYYz3UU36PfH8eU2XbUqa9BvBeMAk4wh+eOdaZULPlcKXE3BSgbwrgYzlGs7SCgBCFe0bmpMw/wVa0EsOtgxvMkHPIAgXkYXXoScgBQKBgQD9xGyU1lp6+R1CWxbwi6Z+ADoFexIOZroF7Jvm9GeahPniIMuYeQDcxlub2Ts3OxFDPzvlWG+mLuNZOcE2AauuN6gZso1aI7chSIJd7w0H5cUgPMOHfkv9hiVjxuHg6Hh2tfoywGNkKXJa27gVMliHu35Yst1F9b4AjYbjM3OdDwKBgQDwUqUv/FAPFqE7iLn/y/ovSfbmUsOTGKHFOHjPAA2ftvHZjRCOdadxSF7ZfKpk1b9qE3NUsNQLGSL7qDtespcI4zqatzeYlEI2EZy7wBku63jvUZRwKF+gpTGCfSyr8RbTKgd5O1181k5lGJ4msDCFGqkDGNAGF33651fNQm7cywKBgHW2Z6eixtXEOI5PMpTiMpjAAioIrJeBj0iwOKEpPeWvSTpbfBV+C6hdWqEwYwd7nsZSzrXUD/ZGSa9Z2wXyQHDP4GoLKQZnCsulltN7z8+aBZaumQSA0T8Ius9Nsflh3H80apqgI3qSvzjmMr2iXO6ZSkJatcl99GTNQYSimT6BAoGBAKdMcVBpbnZCg7WFJi6rmhwHeoi2fw3MrPk1qew4lkNkQRL4v9jckNcs0VLIFXqizmES70u62bJsKk1UvRfcbYYzYV3JRDvEoh8vvZN6VE20gPjhKtu1T3Tu6vtzFoEQEZqo/JDpwdgg/Gwmahp2tyBCAfx07oP/IKVlHVt37uNnAoGARl8NwO2m8QV789rhEq//ods9Wraa81gryeu3xV+Mu7ZdzIphSLfPrj0BmyqGI6G+qBx5WYXBKfdIJz1Qp5eG0mp7BtoCk9XrlDHF//L4J0DrBJg7G0DYLrevhgeULZvIpNL3q7qF13WRnq3uEYZ5UNf/sYObDRrIZoql2sUOzdA=";


        [TestMethod]
        public void TestMethod1()
        {
            string privateKey = "MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDc2I9TZiwiMBQVB9vtDlPU3PJZSC4/lKWRhewbvQihA7yKYg3PyGWBQjvhwALAM2NswOXqDnQkDkxCP4LNTGNr0CuandJJDNO9MWNyT/RdXPov8kItKW8GBcpsVdGavdxLkIOtqL8WqzzDNbN+2iR/bpaopecIn94Zt8VeJa24UA90knHoA0FgsDILlWamajbJFl7hqfK0/ZxaECWsX4Aej1gnaLgfnRuOtFFVgdn4mgwUTk8E5Lggy0n8LdpCLXSnJ6F98ggeZUptE1aOsaa1OwdxXPQ+cpdm1Doab6wkJ9K0aOqqm0X6W9jLsZtGVw9IcqwOkpL9nOrfJKaw1BnbAgMBAAECggEAP91AVVXOnXRZ2lsTdlIWXBAo5TjCZxCNcK/Rv5PC0w1JkJsac2P4NpD40Ce+o+Y4clK2KiFT5LldxCqpoMTeW2cvtN6+2PEkqimGIKCOghFJKGReTsEUoiE6kGmg26bb9d4stiN+XwfV4n29Eony1Ace+lY/oXbZu4YxRTCQNCQpBP3D/qhMd7+JESIcawDxC9qW5w5r5CsBQIBEtITuqO4vbgWY049k/3uJrfN52HI+KQjOQerlALs7zNucRYpwCTbqn5JPwOADVxZI74c0jh7h73GeSfqBzwtorFAovNVzEn5UjmP5PcfAdt1iPEFj4QN+XosQ9KKP7pijsII0wQKBgQD9pysypMHzdWg/0dCF+DXa2jLp2Ach1QxKsgzv8BOBj23COt+ookxuDGGKcTDnpb+4CEeT59XfJkR1HMY6cILMqx/rtB2qIbdwmS3LTQu6qCnIfOywJQnOGdrhpY5r3/K8F5PewMyNfliJOjzhJdM8Tqzpj5zE3+jiG/H3m5VqlQKBgQDe4647GLqCHNPgRzvjNHQrWSxBCZBnzZZB1PIsfE0/iJT90r+lye2vXH6GymtstJga1nD3RHw4rElIY+W3yxedq1JUViLUYWWACjrpu4prRtWYPK8wYh/mrDbx6LYsiBhYULy+zQ5EL33E8VYhaZ/PfDZegrkUr1LutCtpsP3GrwKBgQCEDa7Am+PZYqtWZl4gEPvAp05QTcZQx8Cxdoajt6IAFnK25r2f5nbR6CIz1//06WapYa4aA11l4l9LdSKCNCb0dLaWmFvFQe5lcWnU6JSNpZSzKs5yWaYEJZO9W9qMsLeHUuMJ2efkVf2z25zsPiv9vpvosHqWSFfOt2u+U8JzDQKBgQCihM1D1fIYcUCkZgxqzJJwESNNgNXEeok3EzhJ65C/5K0Orp6DGkuu8/hl/C58IupNl1LjWRJimG0O1ZhofTOJCaSMTgozZZkG1W7DgrWZJxsTWBw9YS71mViu5wn+SnXF34jgbtAaM6627WnqDwQx29Yg75xYUsZbuw90j1NbjQKBgQCgeMNvF6zasDKaCMNwwQ19TeYgJUoYyM3Mlh7zm15NcjEKdsdrNyLzKrdW5XAyAevEBbG6p2sLoxiEXlZCXrDIFx9i2dnBN9Qkb4ZXMlgfZFawQTidalj3Z/G0qriWeyUlk7IAUbyVz40o9m2djG7qc6xEyKVAUvvz9pFJ1fdl7Q==";
            string publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA3NiPU2YsIjAUFQfb7Q5T1NzyWUguP5SlkYXsG70IoQO8imINz8hlgUI74cACwDNjbMDl6g50JA5MQj+CzUxja9Armp3SSQzTvTFjck/0XVz6L/JCLSlvBgXKbFXRmr3cS5CDrai/Fqs8wzWzftokf26WqKXnCJ/eGbfFXiWtuFAPdJJx6ANBYLAyC5Vmpmo2yRZe4anytP2cWhAlrF+AHo9YJ2i4H50bjrRRVYHZ+JoMFE5PBOS4IMtJ/C3aQi10pyehffIIHmVKbRNWjrGmtTsHcVz0PnKXZtQ6Gm+sJCfStGjqqptF+lvYy7GbRlcPSHKsDpKS/Zzq3ySmsNQZ2wIDAQAB";
      
            //string publicKey =
            //    "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoQh0wEqx/R2H1v00IU12Oc30fosRC/frhH89L6G+fzeaqI19MYQhEPMU13wpeqRONCUta+2iC1sgCNQ9qGGf19yGdZUfueaB1Nu9rdueQKXgVurGHJ+5N71UFm+OP1XcnFUCK4wT5d7ZIifXxuqLehP9Ts6sNjhVfa+yU+VjF5HoIe69OJEPo7OxRZcRTe17khc93Ic+PfyqswQJJlY/bgpcLJQnM+QuHmxNtF7/FpAx9YEQsShsGpVo7JaKgLo+s6AFoJ4QldQKir2vbN9vcKRbG3piElPilWDpjXQkOJZhUloh/jd7QrKFimZFldJ1r6Q59QYUyGKZARUe0KZpMQIDAQAB";
            ////2048 Ë½Ô¿
            //string privateKey =
            //    "MIIEpAIBAAKCAQEAoQh0wEqx/R2H1v00IU12Oc30fosRC/frhH89L6G+fzeaqI19MYQhEPMU13wpeqRONCUta+2iC1sgCNQ9qGGf19yGdZUfueaB1Nu9rdueQKXgVurGHJ+5N71UFm+OP1XcnFUCK4wT5d7ZIifXxuqLehP9Ts6sNjhVfa+yU+VjF5HoIe69OJEPo7OxRZcRTe17khc93Ic+PfyqswQJJlY/bgpcLJQnM+QuHmxNtF7/FpAx9YEQsShsGpVo7JaKgLo+s6AFoJ4QldQKir2vbN9vcKRbG3piElPilWDpjXQkOJZhUloh/jd7QrKFimZFldJ1r6Q59QYUyGKZARUe0KZpMQIDAQABAoIBAQCRZLUlOUvjIVqYvhznRK1OG6p45s8JY1r+UnPIId2Bt46oSLeUkZvZVeCnfq9k0Bzb8AVGwVPhtPEDh73z3dEYcT/lwjLXAkyPB6gG5ZfI/vvC/k7JYV01+neFmktw2/FIJWjEMMF2dvLNZ/Pm4bX1Dz9SfD/45Hwr8wqrvRzvFZsj5qqOxv9RPAudOYwCwZskKp/GF+L+3Ycod1Wu98imzMZUH+L5dQuDGg3kvf3ljIAegTPoqYBg0imNPYY/EGoFKnbxlK5S5/5uAFb16dGJqAz3XQCz9Is/IWrOTu0etteqV2Ncs8uqPdjed+b0j8CMsr4U1xjwPQ8WwdaJtTkRAoGBANAndgiGZkCVcc9975/AYdgFp35W6D+hGQAZlL6DmnucUFdXbWa/x2rTSEXlkvgk9X/PxOptUYsLJkzysTgfDywZwuIXLm9B3oNmv3bVgPXsgDsvDfaHYCgz0nHK6NSrX2AeX3yO/dFuoZsuk+J+UyRigMqYj0wjmxUlqj183hinAoGBAMYMOBgF77OXRII7GAuEut/nBeh2sBrgyzR7FmJMs5kvRh6Ck8wp3ysgMvX4lxh1ep8iCw1R2cguqNATr1klOdsCTOE9RrhuvOp3JrYzuIAK6MpH/uBICy4w1rW2+gQySsHcH40r+tNaTFQ7dQ1tef//iy/IW8v8i0t+csztE1JnAoGABdtWYt8FOYP688+jUmdjWWSvVcq0NjYeMfaGTOX/DsNTL2HyXhW/Uq4nNnBDNmAz2CjMbZwt0y+5ICkj+2REVQVUinAEinTcAe5+LKXNPx4sbX3hcrJUbk0m+rSu4G0B/f5cyXBsi9wFCAzDdHgBduCepxSr04Sc9Hde1uQQi7kCgYB0U20HP0Vh+TG2RLuE2HtjVDD2L/CUeQEiXEHzjxXWnhvTg+MIAnggvpLwQwmMxkQ2ACr5sd/3YuCpB0bxV5o594nsqq9FWVYBaecFEjAGlWHSnqMoXWijwu/6X/VOTbP3VjH6G6ECT4GR4DKKpokIQrMgZ9DzaezvdOA9WesFdQKBgQCWfeOQTitRJ0NZACFUn3Fs3Rvgc9eN9YSWj4RtqkmGPMPvguWo+SKhlk3IbYjrRBc5WVOdoX8JXb2/+nAGhPCuUZckWVmZe5pMSr4EkNQdYeY8kOXGSjoTOUH34ZdKeS+e399BkBWIiXUejX/Srln0H4KoHnTWgxwNpTsBCgXu8Q==";
            {
                var rsa = new RsaUtil(privateKey, publicKey);
                //rsa.RsaType = RSAType.RSA;
                //rsa.RsaKeyType = RSAKeyType.PKCS1;

                string str = "²©¿ÍÔ° http://www.cnblogs.com/";
                Console.WriteLine("Ô­Ê¼×Ö·û´®£º" + str);
                //¼ÓÃÜ
                string enStr = rsa.Encrypt(str);
                Console.WriteLine("¼ÓÃÜ×Ö·û´®£º" + enStr);
                //½âÃÜ
                string deStr = rsa.Decrypt(enStr);
                Console.WriteLine("½âÃÜ×Ö·û´®£º" + deStr);
                Assert.AreEqual(str, deStr);
                //Ë½Ô¿Ç©Ãû
                string signStr = rsa.Sign(str);
                Console.WriteLine("×Ö·û´®Ç©Ãû£º" + signStr);
                //¹«Ô¿ÑéÖ¤Ç©Ãû
                Assert.IsTrue(rsa.Verify(str, signStr));
            }
            {
                var rsa = new RsaUtil(MY_PRIVATE_KEY, APIGW_PUBLIC_KEY);

                string str = "²©¿ÍÔ° http://www.cnblogs.com/";
                Console.WriteLine("Ô­Ê¼×Ö·û´®£º" + str);
                //¼ÓÃÜ
                string enStr = rsa.Encrypt(str);
                Console.WriteLine("¼ÓÃÜ×Ö·û´®£º" + enStr);
                //½âÃÜ
                string deStr = rsa.Decrypt(enStr);
                Console.WriteLine("½âÃÜ×Ö·û´®£º" + deStr);
                Assert.AreEqual(str, deStr);
                //Ë½Ô¿Ç©Ãû
                string signStr = rsa.Sign(str);
                Console.WriteLine("×Ö·û´®Ç©Ãû£º" + signStr);
                //¹«Ô¿ÑéÖ¤Ç©Ãû
                Assert.IsTrue(rsa.Verify(str, signStr));
            }

        }
    }
}
