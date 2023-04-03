DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS MarketingInformation;

CREATE TABLE IF NOT EXISTS Users
(
    id    INT,
    name  VARCHAR,
    email VARCHAR,

    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS MarketingInformation
(
    id         INT,
    email      VARCHAR,
    subscribed INT,

    PRIMARY KEY (id)
);


INSERT INTO Users (id, name, email)
VALUES (1, 'Alice', 'alice@email.com');
INSERT INTO Users (id, name, email)
VALUES (2, 'Bob', 'bob@email.com');
INSERT INTO Users (id, name, email)
VALUES (3, 'Charlie', 'charlie@email.com');
INSERT INTO Users (id, name, email)
VALUES (4, 'Dennis', 'dennis@email.com');

INSERT INTO MarketingInformation (id, email, subscribed)
VALUES (1, 'frankenstein@email.com', true);
INSERT INTO MarketingInformation (id, email, subscribed)
VALUES (2, 'bob@email.com', false);
INSERT INTO MarketingInformation (id, email, subscribed)
VALUES (3, 'charlie@email.com', true);
INSERT INTO MarketingInformation (id, email, subscribed)
VALUES (5, 'hans@email.com', false);
INSERT INTO MarketingInformation (id, email, subscribed)
VALUES (6, 'trine@email.com', true);
