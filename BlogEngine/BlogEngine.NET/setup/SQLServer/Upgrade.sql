--
-- Add pagetitle field to pages and posts
--

ALTER TABLE dbo.be_Posts ADD PostTitle nvarchar(255) NULL
GO
ALTER TABLE dbo.be_Pages ADD PostTitle nvarchar(255) NULL
GO