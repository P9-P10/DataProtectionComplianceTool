--- Ikke udfyld rækker, ikke indsæt default værdier

DROP VIEW IF EXISTS MOVE_1;

CREATE VIEW MOVE_1 AS
SELECT U.id, U.name, U.email, subscribed
FROM Users as U
         LEFT JOIN MarketingInformation as M on U.id = M.id;

SELECT * FROM MOVE_1;