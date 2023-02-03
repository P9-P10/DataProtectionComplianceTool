--- Udfyld rækker, destination har præcedens

DROP VIEW IF EXISTS merge_merged_values;
DROP VIEW IF EXISTS merge_temp_relation;
DROP VIEW IF EXISTS merge_to_be_marked;
DROP VIEW IF EXISTS merge_result_relation;
DROP VIEW IF EXISTS merge_marked_users;
DROP VIEW IF EXISTS merge_merged_ids;
DROP VIEW IF EXISTS merge_extended_users;
DROP VIEW IF EXISTS merge_rows_to_be_deleted;
DROP VIEW IF EXISTS merge_temp_merged_values;

CREATE VIEW merge_marked_users AS
SELECT *, CASE WHEN TRUE THEN TRUE END AS mark
FROM Users;

CREATE VIEW merge_merged_ids AS
SELECT id
FROM Users
UNION
SELECT id
FROM MarketingInformation;

CREATE VIEW merge_extended_users AS
SELECT merge_merged_ids.id, U.name, U.email, U.mark
FROM merge_merged_ids FULL OUTER JOIN merge_marked_users as U on merge_merged_ids.id = U.id;

SELECT *
FROM merge_extended_users;

CREATE VIEW merge_temp_merged_values AS
SELECT id, email
FROM Users
UNION
SELECT id, email
FROM MarketingInformation;

CREATE VIEW merge_rows_to_be_deleted AS
SELECT t.id, t.email
FROM (SELECT id, email FROM MarketingInformation EXCEPT SELECT id, email FROM Users) as t
         LEFT JOIN Users ON t.id = Users.id
WHERE Users.id is not null;


CREATE VIEW merge_merged_values AS
SELECT id, email
FROM (SELECT id, email FROM merge_temp_merged_values EXCEPT SELECT id, email FROM merge_rows_to_be_deleted);

CREATE VIEW merge_temp_relation AS
SELECT U.id, name, merge_merged_values.email, U.mark
FROM merge_extended_users as U FULL OUTER JOIN merge_merged_values on U.id = merge_merged_values.id;

SELECT *
FROM merge_temp_relation;

CREATE VIEW merge_result_relation AS
SELECT DISTINCT id,
                name,
                email,
                CASE
                    WHEN merge_temp_relation.mark THEN FALSE
                    ELSE TRUE
                    END AS from_MarketingInformation
FROM merge_temp_relation;

SELECT *
FROM merge_result_relation;
