using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static OmikujiService.OmikujiReply.Types;

namespace OmikujiService
{
    public class OmikujiService : Omikuji.OmikujiBase
    {
        private readonly ILogger<OmikujiService> _logger;
        public OmikujiService(ILogger<OmikujiService> logger)
        {
            _logger = logger;
        }

        public override Task<OmikujiReply> DrawOmikuji(OmikujiRequest request, ServerCallContext context)
        {
            try
            {
                // ここで本来は冪等性チェックをする
                Console.WriteLine($"IdempotencyKey: {request.IdempotencyKey} (idempotency key should be unique in xx seconds)");

                // おみくじ結果
                Result result = draw();
                return Task.FromResult(new OmikujiReply
                {
                    Message = getMessage(result),
                    Result = result,
                });
            }
            finally
            {
                // おみくじ実施イベントをRabbitMQに発行
                new RabbitMQClient().publish("OmikujiDrawn");
            }
        }

        private Result draw()
        {
            Random r = new Random();
            int result = r.Next(0, 5);
            return (Result)result;
        }

        private string getMessage(Result result)
        {
            switch (result)
            {
                case Result.Daikichi: return "大吉";
                case Result.Chukichi: return "中吉";
                case Result.Kichi: return "吉";
                case Result.Shokichi: return "小吉";
                case Result.Kyo: return "凶";
                case Result.Daikyo: return "大凶";
                default: return "";
            }
        }
    }
}
