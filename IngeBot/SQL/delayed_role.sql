

CREATE TABLE delayed_role
(
	id SERIAL PRIMARY KEY,
	guild_id BIGINT,
	owner_id BIGINT,
	target_id BIGINT,
	role_id BIGINT,
	name VARCHAR,
	date_start TIMESTAMP,
	date_end TIMESTAMP
);

