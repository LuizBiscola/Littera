-- Relatório de vendas por cliente (últimos 90 dias)
-- Autor: Luiz · atualizado em 15/11/2026

SET search_path TO vendas, public;

WITH pedidos_recentes AS (
    SELECT p.id_pedido,
           p.id_cliente,
           p.valor_total,
           p.data_pedido
    FROM pedidos p
    WHERE p.data_pedido >= CURRENT_DATE - INTERVAL '90 days'
      AND p.status <> 'CANCELADO'
)
SELECT c.id_cliente,
       c.nome,
       COUNT(pr.id_pedido)   AS qtd_pedidos,
       SUM(pr.valor_total)   AS total_gasto,
       MAX(pr.data_pedido)   AS ultima_compra
FROM clientes c
INNER JOIN pedidos_recentes pr ON pr.id_cliente = c.id_cliente
WHERE c.ativo = TRUE
GROUP BY c.id_cliente, c.nome
HAVING SUM(pr.valor_total) > 1000
ORDER BY total_gasto DESC
LIMIT 50;

/* T-SQL: procedure com variáveis, colchetes e @@ROWCOUNT */
CREATE OR ALTER PROCEDURE dbo.usp_AtualizaVip
    @Limite DECIMAL(18, 2) = 1000.00,
    @Desde  DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Total INT = 0;

    UPDATE c
       SET c.[segmento] = N'VIP'
      FROM [dbo].[clientes] AS c
     WHERE c.id_cliente IN (SELECT TOP (100) id_cliente FROM vw_top_clientes WITH (NOLOCK))
       AND ISNULL(c.data_cadastro, GETDATE()) >= COALESCE(@Desde, DATEADD(DAY, -90, GETDATE()));

    SET @Total = @@ROWCOUNT;
    PRINT CONCAT('Atualizados: ', @Total);
END
GO
