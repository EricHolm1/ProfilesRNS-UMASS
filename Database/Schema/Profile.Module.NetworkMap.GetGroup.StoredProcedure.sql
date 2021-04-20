SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [Profile.Module].[NetworkMap.GetGroup]
	@NodeID BIGINT=NULL,
	@which INT=0,
	@SessionID UNIQUEIDENTIFIER=NULL
AS
BEGIN

	DECLARE @GroupID INT
	SELECT @GroupID = GroupID FROM [Profile.Data].[vwGroup.General] WHERE GroupNodeID = @NodeID

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET nocount  ON;
 
	DECLARE  @f  TABLE(
		PersonID INT,
		display_name NVARCHAR(255),
		latitude FLOAT,
		longitude FLOAT,
		address1 NVARCHAR(1000),
		address2 NVARCHAR(1000),
		URI VARCHAR(400)
	)
 
	INSERT INTO @f (	PersonID,
						display_name,
						latitude,
						longitude,
						address1,
						address2
					)
		SELECT	p.PersonID,
				p.displayname,
				l.latitude,
				l.longitude,
				CASE WHEN p.addressstring like '%,%' THEN LEFT(p.addressstring,CHARINDEX(',',p.addressstring) - 1)ELSE P.addressstring END address1,
				CASE WHEN p.addressstring like '%,%' THEN REPLACE(SUBSTRING(p.addressstring,CHARINDEX(',',p.addressstring) + 1,LEN(p.addressstring)),', USA','') ELSE p.addressstring END address2
		FROM [Profile.Data].vwperson p,
				(SELECT PersonID
					FROM [Profile.Data].[vwGroup.Member]
					WHERE GroupID = @GroupID
					and IsActive = 1
				) t,
				[Profile.Data].vwperson l
		 WHERE p.PersonID = t.PersonID
			 AND p.PersonID = l.PersonID
			 AND l.latitude IS NOT NULL
			 AND l.longitude IS NOT NULL
		 ORDER BY p.lastname, p.firstname
 
	UPDATE @f
		SET URI = p.Value + cast(m.NodeID as varchar(50))
		FROM @f, [RDF.Stage].InternalNodeMap m, [Framework.].Parameter p
		WHERE p.ParameterID = 'baseURI' AND m.InternalHash = [RDF.].fnValueHash(null,null,'http://xmlns.com/foaf/0.1/Person^^Person^^'+cast(PersonID as varchar(50)))
 
	DELETE FROM @f WHERE URI IS NULL
 
 
	IF @which = 0
	BEGIN
		SELECT PersonID, 
			display_name,
			latitude,
			longitude,
			address1,
			address2,
			URI
		FROM @f
		ORDER BY address1,
			address2,
			display_name
	END
	ELSE
	BEGIN
		SELECT DISTINCT	a.latitude	x1,
						a.longitude	y1,
						d.latitude	x2,
						d.longitude	y2,
						a.PersonID	a,
						d.PersonID	b,
						0 is_person,
						a.URI u1,
						d.URI u2
			FROM @f a,
					 [Profile.Data].[Publication.Person.Include] b,
					 [Profile.Data].[Publication.Person.Include] c,
					 @f d
		 WHERE a.PersonID = b.PersonID
			 AND b.pmid = c.pmid
			 AND b.PersonID < c.PersonID
			 AND c.PersonID = d.PersonID
	END
		
END

GO
