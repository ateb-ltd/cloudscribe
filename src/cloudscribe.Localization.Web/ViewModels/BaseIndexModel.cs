﻿namespace cloudscribe.Localization.Web
{
    public class BaseIndexModel<TItem>
    {
        public TItem[] Items { get; internal set; } = null!;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int MaxPage { get; set; } = 1;

        public string? Query { get; set; }

        public string? Order { get; set; }
    }
}
