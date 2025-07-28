/*
 * Copyright 2012-2025 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package org.springframework.samples.petclinic.system;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.nio.charset.StandardCharsets;

import javax.sql.DataSource;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.CommandLineRunner;
import org.springframework.core.io.ClassPathResource;
import org.springframework.dao.DataAccessException;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.stereotype.Component;

/**
 * Database initializer for PetClinic application. Ensures the database schema and data
 * are properly initialized in Docker environments.
 *
 * @author Spring PetClinic Team
 */
@Component
public class DatabaseInitializer implements CommandLineRunner {

	private static final Logger logger = LoggerFactory.getLogger(DatabaseInitializer.class);

	private final JdbcTemplate jdbcTemplate;

	private final String database;

	public DatabaseInitializer(DataSource dataSource, @Value("${database:h2}") String database) {
		this.jdbcTemplate = new JdbcTemplate(dataSource);
		this.database = database;
	}

	@Override
	public void run(String... args) throws Exception {
		logger.info("Initializing database with profile: {}", database);

		// Check if database is already initialized
		if (isDatabaseInitialized()) {
			logger.info("Database already initialized, skipping initialization");
			return;
		}

		try {
			// Initialize schema
			runSqlScript("db/" + database + "/schema.sql");
			logger.info("Database schema initialized successfully");

			// Initialize data
			runSqlScript("db/" + database + "/data.sql");
			logger.info("Database data initialized successfully");

		}
		catch (Exception e) {
			logger.error("Failed to initialize database", e);
			throw e;
		}
	}

	private boolean isDatabaseInitialized() {
		try {
			// Check if the owners table exists and has data
			Integer count = jdbcTemplate.queryForObject("SELECT COUNT(*) FROM owners", Integer.class);
			return count != null && count > 0;
		}
		catch (DataAccessException e) {
			// Table probably doesn't exist yet
			logger.debug("Database not yet initialized: {}", e.getMessage());
			return false;
		}
	}

	private void runSqlScript(String scriptPath) throws IOException {
		logger.info("Running SQL script: {}", scriptPath);

		ClassPathResource resource = new ClassPathResource(scriptPath);
		if (!resource.exists()) {
			logger.warn("SQL script not found: {}", scriptPath);
			return;
		}

		try (BufferedReader reader = new BufferedReader(
				new InputStreamReader(resource.getInputStream(), StandardCharsets.UTF_8))) {

			StringBuilder sqlBuilder = new StringBuilder();
			String line;

			while ((line = reader.readLine()) != null) {
				line = line.trim();

				// Skip empty lines and comments
				if (line.isEmpty() || line.startsWith("--") || line.startsWith("#")) {
					continue;
				}

				sqlBuilder.append(line).append(" ");

				// Execute statement when we hit a semicolon
				if (line.endsWith(";")) {
					String sql = sqlBuilder.toString().trim();
					if (!sql.isEmpty()) {
						try {
							jdbcTemplate.execute(sql);
							logger.debug("Executed: {}", sql.substring(0, Math.min(sql.length(), 50)) + "...");
						}
						catch (DataAccessException e) {
							logger.warn("Failed to execute SQL: {} - Error: {}", sql, e.getMessage());
						}
					}
					sqlBuilder.setLength(0);
				}
			}
		}
	}

}
