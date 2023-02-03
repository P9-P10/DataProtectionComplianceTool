--- Ikke udfyld rækker, destination har ikke præcedens

SELECT t.id, t.name, CASE WHEN t.email is null THEN U2.email ELSE t.email END AS email
FROM (SELECT U1.id, name, M.email
      FROM Users as U1
               LEFT JOIN MarketingInformation as M on U1.id = M.id) as t
         JOIN Users as U2 on t.id = U2.id;