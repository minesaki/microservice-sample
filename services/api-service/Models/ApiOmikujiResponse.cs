using System;

public class ApiOmikujiResponse {
    public string IdempotencyKey {get;set;}
    public DateTime Date {get;set;}
    public string OmikujiResult {get;set;}
    public string Error{get;set;}
}