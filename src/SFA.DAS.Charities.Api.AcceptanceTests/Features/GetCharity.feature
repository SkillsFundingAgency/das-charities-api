Feature: GetCharity
	As a Charities API consumer
	I want to retrieve charity details
	So that I can use these in my application

Scenario: Get charity details by charity registration number
	Given I have a HTTP client
	And I want to retrieve details for charity with registration number 1001
	When I request the following url: charities
	Then a response with HTTP status code of 200 is received
	And the charity with registration number equal to 1001 is returned
