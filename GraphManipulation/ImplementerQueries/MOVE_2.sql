--- Ikke udfyld rækker, indsæt default værdier

DROP VIEW IF EXISTS MOVE_1;
DROP VIEW IF EXISTS MOVE_2;


CREATE VIEW MOVE_1 AS
SELECT U.id, name, U.email, subscribed
FROM Users as U
         LEFT JOIN MarketingInformation as M on U.id = M.id;

CREATE VIEW MOVE_2 AS
SELECT id,
       name,
       email,
       CASE
           WHEN subscribed THEN TRUE
           ELSE FALSE
           END AS subscribed
FROM MOVE_1;

SELECT * FROM MOVE_2;