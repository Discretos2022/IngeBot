

CREATE TABLE delayed_message
(
	id SERIAL PRIMARY KEY,
	guild_id BIGINT,
	owner_id BIGINT,
	channel_id BIGINT,
	name VARCHAR,
	text VARCHAR,
	date TIMESTAMP,
	repeat BOOLEAN
);

