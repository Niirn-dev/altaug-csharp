using AltAug.Domain.Interfaces;

namespace AltAug.Domain.Models.Filters;

public readonly record struct OpenPrefixFilterParameters() : IFilterParams;

public readonly record struct OpenSuffixFilterParameters() : IFilterParams;

public readonly record struct RegexFilterParameters(string RegexString) : IFilterParams;
