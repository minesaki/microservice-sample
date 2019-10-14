﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using OmikujiService;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiService.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;

        public ApiController(ILogger<ApiController> logger)
        {
            _logger = logger;
        }

        [HttpPost("omikuji")]
        public async Task<ApiOmikujiResponse> Omikuji(ApiOmikujiRequest request){
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Omikuji.OmikujiClient(channel);

            try
            {                
                var reply = await client.DrawOmikujiAsync(new OmikujiRequest {IdempotencyKey = request.IdempotencyKey});
                return new ApiOmikujiResponse{
                    IdempotencyKey = request.IdempotencyKey,
                    Date = DateTime.Now,
                    OmikujiResult = $"{request.UserName} さんの運勢は {reply.Message} です。",
                };
            } catch {
                return new ApiOmikujiResponse{
                    IdempotencyKey = request.IdempotencyKey,
                    Date = DateTime.Now,
                    Error = "## おみくじサービスで障害が発生しています ##"
                };
            }
        }
    }
}