--- Udfyld rækker, indsæt default værdier


DROP VIEW IF EXISTS MOVE_3_temp_relation;
DROP VIEW IF EXISTS MOVE_3_temp_temp_relation;
DROP VIEW IF EXISTS MOVE_3;
DROP VIEW IF EXISTS MOVE_3_merged_ids;
DROP VIEW IF EXISTS MOVE_3_extended_users;
DROP VIEW IF EXISTS MOVE_3_marked_users;
DROP VIEW IF EXISTS MOVE_4;

CREATE VIEW MOVE_3_marked_users AS
SELECT *, CASE WHEN TRUE THEN TRUE END AS mark
FROM Users;

CREATE VIEW MOVE_3_merged_ids AS
SELECT id
FROM Users
UNION
SELECT id
FROM MarketingInformation;

CREATE VIEW MOVE_3_extended_users AS
SELECT MOVE_3_merged_ids.id, U.name, U.email, U.mark
FROM MOVE_3_merged_ids FULL OUTER JOIN MOVE_3_marked_users as U on MOVE_3_merged_ids.id = U.id;

SELECT *
FROM MOVE_3_extended_users;

CREATE VIEW MOVE_3_temp_relation AS
SELECT U.id, name, U.email, subscribed, U.mark
FROM MOVE_3_extended_users AS U FULL OUTER JOIN MarketingInformation AS M on U.id = M.id;

SELECT *
FROM MOVE_3_temp_relation;

CREATE VIEW MOVE_3_temp_temp_relation AS
SELECT DISTINCT MOVE_3_temp_relation.id,
                name,
                email,
                subscribed,
                CASE
                    WHEN MOVE_3_temp_relation.mark THEN FALSE
                    ELSE TRUE
                    END AS from_MarketingInformation
FROM MOVE_3_temp_relation;


CREATE VIEW MOVE_3 AS
SELECT M.id,
       name,
       email,
       subscribed,
       from_MarketingInformation,
       CASE WHEN t.id is null THEN FALSE ELSE TRUE END AS originally_in_MarketingInformation
FROM MOVE_3_temp_temp_relation AS M FULL JOIN (SELECT id from MarketingInformation) as t
ON M.id = t.id;

SELECT *
FROM MOVE_3;


CREATE VIEW MOVE_4 AS
SELECT id,
       name,
       email,
       CASE
           WHEN subscribed THEN TRUE
           ELSE FALSE
           END AS subscribed,
       from_MarketingInformation,
       originally_in_MarketingInformation
FROM MOVE_3;

SELECT *
FROM MOVE_4;