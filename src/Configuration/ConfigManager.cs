using DotNetEnv;
using Serilog;

namespace Postbox.Configuration;

/// <summary>
/// Manages environment variables loaded from a local .env file.
/// </summary>
public static class ConfigManager
{
    /// <summary>
    /// Loads the environment variables from the .env file.
    /// </summary>
    public static async Task LoadConfig()
    {
        string env = Environment.GetEnvironmentVariable("DOTNET_ENV_PATH") // for Docker
            ?? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../", ".env"));

        if (!File.Exists(env))
        {
            Log.Error("Dotenv file (.env) does not exist.");
            return;
        }
        try
        {
            string envContent = await File.ReadAllTextAsync(env);
            Env.Load(env);
        }
        catch (Exception ex)
        {
           Log.Error(ex.ToString());
        }
    }

    /// <summary>
    /// Retrieves an environment variable by key.
    /// </summary>
    /// <param name="key">The name of the environment variable.</param>
    /// <param name="defaultValue">The default value to return.</param>
    /// <returns>The value of the environment variable, or an empty value.</returns>
    public static string Get(string key, string defaultValue = "")
    {
        return Env.GetString(key, defaultValue);
    }

    /// <summary>
    /// Retrieves an environment variable as an integer.
    /// </summary>
    /// <param name="key">The name of the environment variable.</param>
    /// <param name="defaultValue">The default value to return if conversion fails.</param>
    /// <returns>The integer value of the environment variable, or the default value if not found.</returns>
    public static int GetInt(string key, int defaultValue = 0)
    {
        string value = Env.GetString(key, defaultValue.ToString());
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        
        return defaultValue;
    }

    /// <summary>
    /// Retrieves an environment variable as a boolean.
    /// </summary>
    /// <param name="key">The name of the environment variable.</param>
    /// <param name="defaultValue">The default value to return if conversion fails.</param>
    /// <returns>The boolean value of the environment variable, or the default value if not found.</returns>
    public static bool GetBool(string key, bool defaultValue = true)
    {
        string value = Env.GetString(key, defaultValue ? "TRUE" : "FALSE").Trim().ToLower();

        return value switch
        {
            "true" => true,
            "false" => false,
            _ => defaultValue
        };
    }
}