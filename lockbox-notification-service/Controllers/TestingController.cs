using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace lockbox_notification_service.Controllers;

[ApiController]
[Route("[controller]")]
public class TestingController : ControllerBase
{
    private readonly ILogger<TestingController> _logger;
    
    public TestingController(ILogger<TestingController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult TestingEndpoint()
    {
        _logger.LogInformation("User reached the testing endpoint.");
        return Ok("Successfully reached the testing API!");
    }

    [HttpGet("simulate-load/{limit}")]
    public ActionResult SimulateLoad(int limit)
    {
        _logger.LogInformation("Starting load simulation...");
        
        Parallel.For(0, Environment.ProcessorCount, i =>
        {
            CpuIntensiveTask.CalculatePrimes(limit);
        });
        
        return Ok("Load simulation complete!");
    }
}

internal static class CpuIntensiveTask
{
    private static bool IsPrime(int number)
    {
        if (number <= 1) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;

        var boundary = (int)Math.Floor(Math.Sqrt(number));

        for (int i = 3; i <= boundary; i += 2)
        {
            if (number % i == 0)
                return false;
        }

        return true;
    }

    public static void CalculatePrimes(int limit)
    {
        var sw = Stopwatch.StartNew();
        int count = 0;

        for (int i = 1; i < limit; i++)
        {
            if (IsPrime(i))
            {
                count++;
            }
        }

        sw.Stop();
        Console.WriteLine($"Found {count} primes in {sw.ElapsedMilliseconds} ms.");
    }
}