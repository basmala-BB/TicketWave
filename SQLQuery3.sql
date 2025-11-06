SELECT * 
FROM movieSubImages
WHERE MovieId NOT IN (SELECT Id FROM movies);

EXEC sp_fkeys 'movieSubImages';



