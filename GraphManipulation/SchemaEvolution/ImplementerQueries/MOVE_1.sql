--- Ikke udfyld rækker, ikke indsæt default værdier

DROP VIEW IF EXISTS MOVE_1_temp;
DROP VIEW IF EXISTS MOVE_1;

CREATE VIEW MOVE_1_temp AS
SELECT U.*, subscribed
FROM Users as U
         LEFT JOIN MarketingInformation as M on U.id = M.id;

SELECT *
FROM MOVE_1_temp;

CREATE VIEW MOVE_1 AS
SELECT M.*,
       CASE WHEN t.id is null THEN FALSE ELSE TRUE END AS originally_in_MarketingInformation
FROM MOVE_1_temp AS M
         LEFT JOIN (SELECT id from MarketingInformation) as t
                   ON M.id = t.id;

SELECT *
FROM MOVE_1;