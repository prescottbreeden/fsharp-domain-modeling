DROP SCHEMA IF EXISTS test_users;
CREATE SCHEMA test_users;
USE test_users;

CREATE TABLE address (
    id          INTEGER       NOT NULL  AUTO_INCREMENT  PRIMARY KEY,
    city        VARCHAR(50)   NOT NULL,
    state       VARCHAR(50)   NOT NULL,
    street1     VARCHAR(100)  NOT NULL,
    street2     VARCHAR(100)  NOT NULL,
    zip_code    VARCHAR(50)   NOT NULL
);

CREATE TABLE teacher_credential (
    id                      INTEGER     NOT NULL  AUTO_INCREMENT  PRIMARY KEY,
    credential_area         VARCHAR(50) NOT NULL,
    credential_expiration   DATETIME    NOT NULL,
    credential_number       VARCHAR(50) NOT NULL,
    is_teacher              TINYINT(1)  NOT NULL
);

CREATE TABLE email (
    id              INTEGER         NOT NULL  AUTO_INCREMENT  PRIMARY KEY,
    email           VARCHAR(100)    NOT NULL,
    email_verified  TINYINT(1)      NOT NULL DEFAULT 0
);

CREATE TABLE phone (
    id              INTEGER         NOT NULL  AUTO_INCREMENT  PRIMARY KEY,
    phone           VARCHAR(10)     NOT NULL
);

CREATE TABLE user (
    id                      INTEGER         NOT NULL  AUTO_INCREMENT  PRIMARY KEY,
    birthdate               DATETIME        NULL,
    first_name              VARCHAR(50)     NOT NULL,
    last_name               VARCHAR(50)     NOT NULL,
    middle_name             VARCHAR(50)     NULL,
    password                VARCHAR(100)    NOT NULL,
    address_id              INTEGER         NULL,
    teacher_credential_id   INTEGER         NULL,
    modified_by             INTEGER         NULL,
    created_at              TIMESTAMP       NOT NULL  DEFAULT NOW(),
    updated_at              TIMESTAMP       NOT NULL  DEFAULT NOW()   ON UPDATE NOW(),

    FOREIGN KEY (address_id)
        REFERENCES address (id),

    FOREIGN KEY (teacher_credential_id)
        REFERENCES teacher_credential (id)
);

CREATE TABLE user_email (
    id          INTEGER     NOT NULL  AUTO_INCREMENT  PRIMARY KEY,
    user_id     INTEGER     NOT NULL,
    email_id    INTEGER     NOT NULL,

    FOREIGN KEY (user_id)
        REFERENCES user (id),

    FOREIGN KEY (email_id)
        REFERENCES email (id)
);

CREATE TABLE user_phone (
    id          INTEGER     NOT NULL  AUTO_INCREMENT  PRIMARY KEY,
    user_id     INTEGER     NOT NULL,
    phone_id    INTEGER     NOT NULL,

    FOREIGN KEY (user_id)
        REFERENCES user (id),

    FOREIGN KEY (phone_id)
        REFERENCES phone (id)
);
