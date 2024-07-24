using Bogus;
using RandomDataGenerator.Models;

namespace RandomDataGenerator.Service;

public class UserDataService
{
    private readonly Dictionary<string, Faker<UserData>> _regionFakers;

    public UserDataService()
    {
        _regionFakers = new Dictionary<string, Faker<UserData>>
        {
            { "Poland", CreatePolandFaker() },
            { "USA", CreateUSAFaker() },
            { "Georgia", CreateGeorgiaFaker() }
        };
    }

    public List<UserData> GenerateData(string region, int errors, int seed, int pageNumber, int pageSize)
    {
        Randomizer.Seed = new Random(seed + pageNumber);
        
        var faker = _regionFakers[region];
        var users = faker.Generate(pageSize);
        
        ApplyErrors(users, errors);
        
        return users;
    }

    private Faker<UserData> CreatePolandFaker()
    {
        return new Faker<UserData>("pl")
            .RuleFor(u => u.Index, f => f.IndexFaker)
            .RuleFor(u => u.Identifier, f => Guid.NewGuid().ToString())
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Address, f => $"{f.Address.City()}, {f.Address.StreetAddress()} {f.Address.BuildingNumber()}")
            .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber());
    }

    private Faker<UserData> CreateUSAFaker()
    {
        return new Faker<UserData>("en")
            .RuleFor(u => u.Index, f => f.IndexFaker)
            .RuleFor(u => u.Identifier, f => Guid.NewGuid().ToString())
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Address, f => $"{f.Address.City()}, {f.Address.StreetAddress()} {f.Address.ZipCode()}")
            .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber());
    }

    private Faker<UserData> CreateGeorgiaFaker()
    {
        return new Faker<UserData>("ge")
            .RuleFor(u => u.Index, f => f.IndexFaker)
            .RuleFor(u => u.Identifier, f => Guid.NewGuid().ToString())
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Address, f => $"{f.Address.City()}, {f.Address.StreetAddress()} {f.Address.BuildingNumber()}")
            .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber());
    }

    private void ApplyErrors(List<UserData> users, int errors)
    {
        Random random = new();
        
        foreach (var user in users)
        {
            for (int i = 0; i < errors; i++)
            {
                ApplyRandomError(user, random);
            }
        }
    }

    private void ApplyRandomError(UserData user, Random random)
    {
        var properties = new[] { "Name", "Address", "Phone" };
        var property = properties[random.Next(properties.Length)];

        switch (property)
        {
            case "Name":
                user.Name = ApplyErrorToString(user.Name, random);
                break;
            case "Address":
                user.Address = ApplyErrorToString(user.Address, random);
                break;
            case "Phone":
                user.Phone = ApplyErrorToString(user.Phone, random);
                break;
        }
    }

    private string ApplyErrorToString(string input, Random random)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        int errorType = random.Next(3);

        switch (errorType)
        {
            case 0: 
                if (input.Length > 1)
                {
                    int pos = random.Next(input.Length);
                    
                    input = input.Remove(pos, 1);
                }
                break;
            case 1: 
                int insertPos = random.Next(input.Length + 1);
                char randomChar = (char)random.Next('a', 'z' + 1);
                
                input = input.Insert(insertPos, randomChar.ToString());
                break;
            case 2: 
                if (input.Length > 1)
                {
                    int swapPos = random.Next(input.Length - 1);
                    char temp = input[swapPos];
                    char[] chars = input.ToCharArray();
                    
                    chars[swapPos] = chars[swapPos + 1];
                    chars[swapPos + 1] = temp;
                    input = new string(chars);
                }
                break;
        }

        return input;
    }
}