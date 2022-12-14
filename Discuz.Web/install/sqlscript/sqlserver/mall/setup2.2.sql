CREATE PROCEDURE [dnt_creategoodstags]
@tags nvarchar(55),
@goodsid int,
@userid int,
@postdatetime datetime
AS
BEGIN
	exec [dnt_createtags] @tags, @userid, @postdatetime

	UPDATE [dnt_tags] SET [gcount]=[gcount]+1,[count]=[count]+1
	WHERE EXISTS (SELECT [item] FROM [dnt_split](@tags, ' ') AS [newtags] WHERE [newtags].[item] = [tagname])

	INSERT INTO [dnt_goodstags] (tagid, goodsid)
	SELECT tagid, @goodsid FROM [dnt_tags] WHERE EXISTS (SELECT * FROM [dnt_split](@tags, ' ') WHERE [item] = [dnt_tags].[tagname])
END
GO

CREATE PROCEDURE [dnt_getgoodscount]
@condition varchar(500)
AS
DECLARE @strSQL varchar(5000)
SET @strSQL = 'SELECT count(goodsid) FROM [dnt_goods] WHERE '+ @condition
EXEC(@strSQL)
GO

CREATE PROCEDURE [dnt_getgoodscountByCid]
@categoryid int,
@condition varchar(500)
AS
DECLARE @strSQL varchar(5000)
SET @strSQL = 'SELECT count(goodsid) FROM [dnt_goods] WHERE ([categoryid] = '+STR(@categoryid)+' OR CHARINDEX('',''+CAST('+STR(@categoryid)+' AS VARCHAR(10))+'','' , '',''+RTRIM([parentcategorylist])+'','')>0)  '+ @condition
EXEC(@strSQL)
GO

CREATE PROCEDURE [dnt_getgoodslist]
@categoryid int,
@pagesize int,
@pageindex int,
@condition varchar(500),
@orderby varchar(100),
@ascdesc int
AS

DECLARE @strSQL varchar(5000)
DECLARE @sorttype varchar(5)

IF @ascdesc=0
   SET @sorttype='ASC'
ELSE
   SET @sorttype='DESC'

IF @pageindex <= 1
	SET @strSQL = 'SELECT TOP '+STR(@pagesize)+' * FROM [dnt_goods] WHERE  '+ @condition +' ORDER BY '+@orderby+' '+@sorttype
ELSE
	IF @sorttype = 'DESC'
		SET @strSQL = 'SELECT TOP '+STR(@pagesize)+' * FROM [dnt_goods] WHERE [goodsid] < (SELECT MIN([goodsid])  FROM (SELECT TOP '+ STR((@pageindex - 1) * @pagesize) + ' [goodsid] FROM [dnt_goods]  WHERE  '+ @condition +' ORDER BY '+@orderby+' '+@sorttype+') AS tblTmp ) AND '+@condition+' ORDER BY '+@orderby+' '+@sorttype
	ELSE
		SET @strSQL = 'SELECT TOP '+STR(@pagesize)+' * FROM [dnt_goods] WHERE [goodsid] > (SELECT MAX([goodsid])  FROM (SELECT TOP '+ STR((@pageindex - 1) * @pagesize) + ' [goodsid] FROM [dnt_goods]  WHERE  '+ @condition +' ORDER BY '+@orderby+' '+@sorttype+') AS tblTmp ) AND '+@condition+' ORDER BY '+@orderby+' '+@sorttype	
EXEC(@strSQL)
GO

CREATE PROCEDURE [dnt_getgoodslistByCid]
@categoryid int,
@pagesize int,
@pageindex int,
@condition varchar(500),
@orderby varchar(100),
@ascdesc int
AS

DECLARE @strSQL varchar(5000)
DECLARE @sorttype varchar(5)

IF @ascdesc=0
   SET @sorttype='ASC'
ELSE
   SET @sorttype='DESC'

IF @pageindex <= 1
	SET @strSQL = 'SELECT TOP '+STR(@pagesize)+' * FROM [dnt_goods] WHERE ([categoryid] = '+STR(@categoryid)+' OR CHARINDEX('',''+CAST('+STR(@categoryid)+' AS VARCHAR(10))+'','' , '',''+RTRIM([parentcategorylist])+'','')>0)  '+ @condition +' ORDER BY '+@orderby+' '+@sorttype
ELSE
	IF @sorttype = 'DESC'
		SET @strSQL = 'SELECT TOP '+STR(@pagesize)+' * FROM [dnt_goods] WHERE [goodsid] < (SELECT MIN([goodsid])  FROM (SELECT TOP '+ STR((@pageindex - 1) * @pagesize) + ' [goodsid] FROM [dnt_goods]  WHERE  ([categoryid] = '+STR(@categoryid)+' OR CHARINDEX('',''+CAST('+STR(@categoryid)+' AS VARCHAR(10))+'','' , '',''+RTRIM([parentcategorylist])+'','')>0)  '+ @condition +' ORDER BY '+@orderby+' '+@sorttype+') AS tblTmp ) AND ([categoryid] = '+STR(@categoryid)+' OR CHARINDEX('',''+CAST('+STR(@categoryid)+' AS VARCHAR(10))+'','' , '',''+RTRIM([parentcategorylist])+'','')>0) '+@condition+' ORDER BY '+@orderby+' '+@sorttype
	ELSE
		SET @strSQL = 'SELECT TOP '+STR(@pagesize)+' * FROM [dnt_goods] WHERE [goodsid] > (SELECT MAX([goodsid])  FROM (SELECT TOP '+ STR((@pageindex - 1) * @pagesize) + ' [goodsid] FROM [dnt_goods]  WHERE  ([categoryid] = '+STR(@categoryid)+' OR CHARINDEX('',''+CAST('+STR(@categoryid)+' AS VARCHAR(10))+'','' , '',''+RTRIM([parentcategorylist])+'','')>0) '+ @condition +' ORDER BY '+@orderby+' '+@sorttype+') AS tblTmp ) AND ([categoryid] = '+STR(@categoryid)+' OR CHARINDEX('',''+CAST('+STR(@categoryid)+' AS VARCHAR(10))+'','' , '',''+RTRIM([parentcategorylist])+'','')>0) '+@condition+' ORDER BY '+@orderby+' '+@sorttype	
EXEC(@strSQL)


