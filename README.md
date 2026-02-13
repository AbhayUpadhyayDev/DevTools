
# DevTools

[![NuGet](https://img.shields.io/nuget/v/DevTools.svg)](https://www.nuget.org/packages/DevTools)&nbsp;
[![GitHub stars](https://img.shields.io/github/stars/AbhayUpadhyayDev/DevTools?style=social)](https://github.com/AbhayUpadhyayDev/DevTools/stargazers)&nbsp;
[![GitHub forks](https://img.shields.io/github/forks/AbhayUpadhyayDev/DevTools?style=social)](https://github.com/AbhayUpadhyayDev/DevTools/network/members)&nbsp;
[![GitHub issues](https://img.shields.io/github/issues/AbhayUpadhyayDev/DevTools)](https://github.com/AbhayUpadhyayDev/DevTools/issues)&nbsp;
[![GitHub license](https://img.shields.io/github/license/AbhayUpadhyayDev/DevTools)](https://github.com/AbhayUpadhyayDev/DevTools/blob/main/LICENSE)&nbsp;


**DevTools** is a **lightweight, all-in-one C# utility library** that makes everyday development tasks faster and simpler. It combines **50+ methods per utility class** for string manipulation, dates, math, collections, file I/O, randomization, URLs, JSON, reflection, processes, crypto, validation, and more—all in a single package.

---

## Features

### ✅ String Utilities
- Convert between **CamelCase, PascalCase, snake_case, Title Case**
- Reverse, truncate, repeat, remove whitespace, digits, and special characters
- Palindrome checks, word counts, substring helpers, regex helpers
- 50+ advanced string manipulation methods  

### ✅ Time Utilities
- Current Unix timestamp, time ago formatting  
- Start/End of day, month, year  
- Add/Subtract days, hours  
- Weekend/Weekday checks, days until, seconds since  
- 50+ advanced time/date helpers  

### ✅ Math Utilities
- Clamp, Lerp, RandomInt, Factorial, IsPrime  
- GCD, LCM, Radians/Degrees conversion  
- RoundToNearest, Percentages, Randomization helpers  
- 100+ mathematical helper methods  

### ✅ Collection Utilities
- Shuffle, chunk, distinct by, remove nulls  
- Safe first/last, concat, reverse, to dictionary  
- 50+ collection manipulation helpers  

### ✅ File Utilities
- Safe read/write/copy/delete, file existence checks  
- Read/Write JSON or text safely  
- 50+ file handling methods  

### ✅ Random Utilities
- Random string, int, bool, double, enum, array element  
- Secure random key generation  
- 50+ randomization helpers  

### ✅ Url Utilities
- Validation, domain/subdomain/TLD extraction  
- Query string manipulation, fragment handling  
- Encoding/decoding, URL normalization, scheme modification  
- 50+ URL utilities  

### ✅ Json Utilities
- Serialize/Deserialize generic and non-generic  
- Pretty print, minify, merge, deep merge  
- Safe property retrieval, case formatting  
- 50+ JSON helpers  

### ✅ Other Utilities
- Reflection, Process management, Crypto, Validation, Logger  
- Color manipulation, Console helpers, Network utilities  
- Enum helpers, Conversion, Threading, Date helpers  

---

## Installation

Install via **NuGet**:

```bash
Install-Package DevTools
````

Or via **.NET CLI**:

```bash
dotnet add package DevTools
```

---

## Usage Examples

### String Helpers

```csharp
using DevTools;

string str = "HelloWorld";
Console.WriteLine(str.ToSnakeCase()); // hello_world
Console.WriteLine(str.IsPalindrome()); // false
```

### Time Helpers

```csharp
var unix = TimeUtils.CurrentUnixTimestamp();
var ago = TimeUtils.TimeAgo(DateTime.UtcNow.AddHours(-5));
```

### Math Helpers

```csharp
int clamped = MathUtils.Clamp(15, 0, 10); // 10
double radians = MathUtils.DegreesToRadians(90); // 1.5707...
```

### File Helpers

```csharp
FileUtils.WriteAllTextSafe("test.txt", "Hello DevTools!");
var content = FileUtils.ReadAllTextSafe("test.txt");
```

### Json Helpers

```csharp
string json = JsonUtils.Serialize(new { Name = "DevTools", Version = 1 });
var obj = JsonUtils.Deserialize<Dictionary<string, object>>(json);
```

---

## Stats

| Class           | Methods |
| --------------- | ------- |
| StringUtils     | 50+     |
| TimeUtils       | 50+     |
| MathUtils       | 100+    |
| CollectionUtils | 50+     |
| FileUtils       | 50+     |
| RandomUtils     | 50+     |
| UrlUtils        | 50+     |
| JsonUtils       | 50+     |
| ReflectionUtils | 10+     |
| ProcessUtils    | 10+     |
| CryptoUtils     | 10+     |
| ValidationUtils | 10+     |
| LoggerUtils     | 10+     |
| ColorUtils      | 10+     |
| ConsoleUtils    | 10+     |
| NetworkUtils    | 10+     |
| EnumUtils       | 10+     |
| ConversionUtils | 10+     |
| ThreadingUtils  | 10+     |

*Total methods: 600+ and growing!*

---

## Contributing

Contributions are welcome!

1. Fork the repository
2. Create a new branch for your feature/fix
3. Submit a pull request

---

> DevTools — a **one-stop utility library** for C# developers, saving hundreds of lines of boilerplate code every day.
