-- 1️⃣ شوف اسم الـ FK القديم لو مش متأكد
SELECT 
    CONSTRAINT_NAME
FROM 
    INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE 
    TABLE_NAME = 'movies' 
    AND CONSTRAINT_TYPE = 'FOREIGN KEY';

-- 2️⃣ احذف الـ FK القديم
ALTER TABLE movies 
DROP CONSTRAINT FK_movies_actors_actorsId;  -- استبدل بالاسم الصحيح لو مختلف

-- 3️⃣ احذف الأعمدة المباشرة لو موجودة
ALTER TABLE movies 
DROP COLUMN ActorId;

ALTER TABLE movies 
DROP COLUMN actorsId;
