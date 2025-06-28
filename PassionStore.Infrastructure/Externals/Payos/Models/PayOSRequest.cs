using Net.payOS.Types;
using System.Security.Cryptography;
using System.Text;

public record PayOSRequest(
    int Amount,
    string Description,
    int OrderCode,
    List<ItemData> Items)
{
    public string? Signature { get; init; }

    public PayOSRequest WithSignature(string secretKey, string returnUrl, string cancelUrl)
    {
        if (string.IsNullOrEmpty(secretKey))
            throw new ArgumentNullException(nameof(secretKey));
        if (Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(Amount));
        if (string.IsNullOrEmpty(returnUrl))
            throw new ArgumentNullException(nameof(returnUrl));
        if (string.IsNullOrEmpty(cancelUrl))
            throw new ArgumentNullException(nameof(cancelUrl));
        if (Items == null || !Items.Any())
            throw new ArgumentException("Items cannot be empty.", nameof(Items));

        var data = new Dictionary<string, string>
        {
            { "amount", Amount.ToString() },
            { "cancelUrl", cancelUrl },
            { "description", Description ?? "Thanh toán đơn hàng" },
            { "orderCode", OrderCode.ToString() },
            { "returnUrl", returnUrl }
        };

        var sortedString = string.Join("&", data.OrderBy(k => k.Key).Select(k => $"{k.Key}={k.Value}"));
        Console.WriteLine($"Signature data string: {sortedString}"); // Debug log

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(sortedString));
        var signature = string.Concat(hash.Select(b => b.ToString("x2")));
        Console.WriteLine($"Generated signature: {signature}"); // Debug log

        return this with { Signature = signature };
    }
}