--- Ikke udfyld rækker, indsæt default værdier

DROP VIEW IF EXISTS MOVE_1;
DROP VIEW IF EXISTS MOVE_1_temp;
DROP VIEW IF EXISTS MOVE_2;


CREATE VIEW MOVE_1_temp AS
SELECT U.id, U.name, U.email, subscribed
FROM Users as U
         LEFT JOIN MarketingInformation as M on U.id = M.id;

SELECT *
FROM MOVE_1_temp;

CREATE VIEW MOVE_1 AS
SELECT M.id,
       name,
       email,
       subscribed,
       CASE WHEN t.id is null THEN FALSE ELSE TRUE END AS originally_in_MarketingInformation
FROM MOVE_1_temp AS M
         LEFT JOIN (SELECT id from MarketingInformation) as t
                   ON M.id = t.id;

CREATE VIEW MOVE_2 AS
SELECT id,
       name,
       email,
       CASE
           WHEN subscribed THEN TRUE
           ELSE FALSE
           END AS subscribed,
       originally_in_MarketingInformation
FROM MOVE_1;

SELECT *
FROM MOVE_2;