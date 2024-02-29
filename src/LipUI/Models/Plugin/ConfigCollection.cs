using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LipUI.Models.Plugin;

public struct ConfigItem
{
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonIgnore]
    public readonly bool IsNull => Value is "null";

    public readonly T As<T>(IFormatProvider? formatProvider = null)
        where T : IParsable<T>
        => T.Parse(Value, formatProvider);

    public static implicit operator ConfigItem(string value) => new() { Value = value };

    public static implicit operator ConfigItem(int value) => new() { Value = value.ToString() };

    public static implicit operator ConfigItem(double value) => new() { Value = value.ToString() };

    public static implicit operator ConfigItem(bool value) => new() { Value = value.ToString() };

    public static implicit operator ConfigItem(Guid value) => new() { Value = value.ToString() };

    public static implicit operator ConfigItem(DateTime value) => new() { Value = value.ToString() };

    public static implicit operator ConfigItem(TimeSpan value) => new() { Value = value.ToString() };

    public static implicit operator ConfigItem(Version value) => new() { Value = value.ToString() };

    public static implicit operator ConfigItem(Uri value) => new() { Value = value.ToString() };

    public static implicit operator string(ConfigItem item) => item.Value;

    public static ConfigItem Create<T>(T value)
        where T : IParsable<T>
    => new() { Value = value.ToString() ?? "null" };

    public override readonly string ToString() => Value;
}

public class ConfigCollection
    : IDictionary<string, ConfigItem>
    , INotifyCollectionChanged
{
    private readonly Dictionary<string, ConfigItem> items;
    internal bool Changed { get; set; } = false;

    internal ConfigCollection()
    {
        items = [];
        CollectionChanged += ConfigCollection_CollectionChanged;
    }

    internal ConfigCollection(string json)
    {
        items = JsonSerializer.Deserialize<Dictionary<string, ConfigItem>>(json) ?? throw new ArgumentNullException(nameof(json));
        CollectionChanged += ConfigCollection_CollectionChanged;
    }

    public ConfigItem this[string key]
    {
        get
        {
            if (items.TryGetValue(key, out var value))
                return value;

            var item = new ConfigItem() { Value = "null" };
            items.Add(key, item);
            return item;
        }
        set
        {
            items[key] = value;
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
        }
    }

    public ICollection<string> Keys => items.Keys;

    public ICollection<ConfigItem> Values => items.Values;

    public int Count => items.Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<string, ConfigItem>>)items).IsReadOnly;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    private void ConfigCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Changed = true;
        Main.ConfigChanged();
    }

    public void Add(string key, ConfigItem value)
    {
        items.Add(key, value);
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
    }

    public void Add(KeyValuePair<string, ConfigItem> item)
    {
        ((ICollection<KeyValuePair<string, ConfigItem>>)items).Add(item);
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
    }

    public void Clear()
    {
        ((ICollection<KeyValuePair<string, ConfigItem>>)items).Clear();
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
    }

    public bool Contains(KeyValuePair<string, ConfigItem> item)
        => ((ICollection<KeyValuePair<string, ConfigItem>>)items).Contains(item);

    public bool ContainsKey(string key)
        => items.ContainsKey(key);

    public void CopyTo(KeyValuePair<string, ConfigItem>[] array, int arrayIndex)
        => ((ICollection<KeyValuePair<string, ConfigItem>>)items).CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<string, ConfigItem>> GetEnumerator()
        => items.GetEnumerator();

    public bool Remove(string key)
    {
        var ret = ((IDictionary<string, ConfigItem>)items).Remove(key);
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove));
        return ret;
    }

    public bool Remove(KeyValuePair<string, ConfigItem> item)
    {
        var ret = ((ICollection<KeyValuePair<string, ConfigItem>>)items).Remove(item);
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove));
        return ret;
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out ConfigItem value)
        => items.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator()
        => items.GetEnumerator();

    internal string Serialize() => JsonSerializer.Serialize(items, Config.options);

    internal static ConfigCollection Deserialize([StringSyntax("Json")] string json) => new(json);
}
