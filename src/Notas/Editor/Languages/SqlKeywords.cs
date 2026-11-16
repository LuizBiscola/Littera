namespace Notas.Editor.Languages;

/// <summary>
/// Listas de palavras para o realce de SQL (ANSI + T-SQL + o comum de PostgreSQL/MySQL).
/// Os lexers da Lexilla comparam em minúsculas, por isso tudo aqui é minúsculo.
/// </summary>
internal static class SqlKeywords
{
    public const string Keywords =
        "absolute action add after all alter and any apply as asc at authorization backup before begin between " +
        "break browse bulk by cascade case catch check checkpoint close clustered collate column commit compute " +
        "constraint contains containstable continue create cross current current_date current_time " +
        "current_timestamp current_user cursor database dbcc deallocate declare default deferrable delete deleted " +
        "deny desc disk distinct distributed do domain drop dump else end errlvl escape except exec execute exists " +
        "exit extension external false fetch file fillfactor filter first following for foreign freetext " +
        "freetexttable from full function go goto grant group having holdlock identity identity_insert identitycol " +
        "if ilike in index inner insert inserted instead intersect interval into is join key kill language last " +
        "lateral left level like limit lineno load loop matched materialized merge national natural next no nocheck " +
        "nocount nonclustered not nowait null nulls of off offset offsets on only open opendatasource openquery " +
        "openrowset openxml option or order outer output over partition percent perform pivot plan preceding " +
        "primary print prior proc procedure public raise raiserror range read readtext reconfigure recursive " +
        "references relative rename replace replication restore restrict return returning returns revert revoke " +
        "right rollback row rowcount rowguidcol rows rule save schema securityaudit select sequence " +
        "session_user set setuser show shutdown similar some statistics system_user table tablesample temp " +
        "temporary textsize then throw ties to top tran transaction trigger true truncate try tsequal type " +
        "unbounded union unique unknown unpivot update updatetext use user using values varying view waitfor when " +
        "where while window with within without work writetext xact_abort zone";

    public const string DataTypes =
        "bigint bigserial binary bit bool boolean bytea char character date datetime datetime2 datetimeoffset dec " +
        "decimal double float geography geometry hierarchyid image int int2 int4 int8 integer json jsonb money " +
        "nchar ntext number numeric nvarchar nvarchar2 precision real rowversion serial smalldatetime smallint " +
        "smallmoney smallserial sql_variant sysname text time timestamp timestamptz tinyint uniqueidentifier uuid " +
        "varbinary varchar varchar2 xml";

    public const string Functions =
        "abs array_agg avg cast ceiling ceil charindex choose coalesce concat concat_ws convert count count_big " +
        "cume_dist datalength date_part date_trunc dateadd datediff datediff_big datefromparts datename datepart day " +
        "db_id db_name dense_rank eomonth exp extract first_value floor format getdate getutcdate " +
        "generate_series greatest grouping iif isdate isnull isnumeric json_agg json_query json_value lag last_value " +
        "lead least len length log lower lpad ltrim max min month newid newsequentialid now ntile nullif nvl " +
        "object_id object_name patindex percent_rank position power quotename rand rank regexp_replace " +
        "replicate reverse round row_number rpad rtrim scope_identity sign space split_part sqrt square stdev " +
        "str string_agg string_split stuff substring sum sysdatetime sysdatetimeoffset sysutcdatetime to_char " +
        "to_date to_timestamp translate trim try_cast try_convert try_parse upper var year";
}
