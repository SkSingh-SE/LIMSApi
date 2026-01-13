# Implementation Plan: LIMS Dashboard Backend

## Overview

This implementation plan breaks down the LIMS Dashboard backend feature into discrete coding tasks that build incrementally. Each task focuses on specific components while ensuring integration with the existing ASP.NET Core system. The implementation follows the established patterns in the LIMS codebase and maintains backward compatibility.

## Tasks

- [x] 1. Create Dashboard DTOs and Data Models
  - Create `DashboardResponseDto`, `DashboardCardDto`, `DashboardChartDto`, and `DashboardNotificationDto` classes
  - Add `ChartDataPointDto` and `DashboardErrorResponse` supporting classes
  - Place DTOs in the existing `LIMSApi/Dtos` directory following established naming conventions
  - Ensure all DTOs are serializable and include proper validation attributes
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

- [x] 1.1 Write property test for DTO structure validation
  - **Property 8: DTO structure consistency**
  - **Validates: Requirements 8.1, 8.5**

- [-] 2. Create Dashboard Service Interface and Implementation
  - [x] 2.1 Create `IDashboardService` interface in `ServiceWORepo` directory
    - Define `GetDashboardAsync`, `GetDashboardCardsAsync`, `GetDashboardChartsAsync`, and `GetDashboardNotificationsAsync` methods
    - Follow existing service interface patterns in the codebase
    - _Requirements: 1.1, 1.2_

  - [x] 2.2 Implement `DashboardService` class with core structure
    - Create service class with dependency injection for `LIMSContext` and `ILogger`
    - Implement basic service methods with placeholder logic
    - Add proper error handling and logging infrastructure
    - _Requirements: 1.1, 1.4, 1.5, 10.1, 10.4_

- [x] 2.3 Write property test for read-only data access
  - **Property 4: Read-only data access**
  - **Validates: Requirements 1.4, 1.5**

- [x] 3. Implement Role-Based Data Filtering
  - [x] 3.1 Add user context validation logic
    - Implement validation for `LoggedInUserDTO` parameter
    - Add role detection and validation methods
    - Handle invalid or missing user context scenarios
    - _Requirements: 2.5_

  - [x] 3.2 Implement role-based card filtering
    - Create methods to filter dashboard cards based on user role
    - Implement Admin, Accounts, and Normal user role logic
    - Ensure billing data is excluded for Normal users
    - _Requirements: 2.1, 2.2, 2.3, 2.4_


- [x] 4. Implement Operational Dashboard Cards
  - [x] 4.1 Implement "Pending Sample Inward" card logic
    - Query `SampleInward` table for samples with `InwardStatus = "Sample Received"`
    - Calculate counts and apply role-based filtering
    - _Requirements: 3.1_

  - [x] 4.2 Implement "Today's Samples" card logic
    - Query `SampleInward` table for samples where `CollectionTime` is today
    - Handle timezone considerations and date filtering
    - _Requirements: 3.2_

  - [x] 4.3 Implement remaining operational cards
    - Add "Overdue Samples", "Pending Plan Approval", "Samples Under Testing", "Results Pending Review", and "Reports Pending Dispatch" cards
    - Use appropriate status fields and date comparisons for each card
    - _Requirements: 3.3, 3.4, 3.5, 3.6, 3.7_

  - [x] 4.4 Implement urgent sample highlighting
    - Add logic to detect urgent/express samples using the `Urgent` flag
    - Include special status indicators in card metadata
    - _Requirements: 3.8, 3.9_




- [x] 5. Implement Billing Dashboard Cards
  - [x] 5.1 Implement "Pending Invoices" card logic
    - Query `TaxInvoice` table for invoices with `Status = "Generated"`
    - Calculate pending invoice counts and amounts
    - _Requirements: 4.1_

  - [x] 5.2 Implement "Paid Invoices" and "Payment Summary" cards
    - Query `PaymentOrder` table for payments with `Status = "Paid"`
    - Calculate payment summaries and totals from existing tables
    - _Requirements: 4.2, 4.3, 4.4_

- [x] 6. Checkpoint - Ensure core dashboard functionality works
  - Ensure all tests pass, ask the user if questions arise.

- [x] 7. Implement Chart Data Aggregation
  - [x] 7.1 Implement daily sample inward trend chart
    - Aggregate `SampleInward` data by `CollectionTime` for trend analysis
    - Create `ChartDataPointDto` objects with date and count values
    - _Requirements: 6.1_

  - [x] 7.2 Implement testing completion status chart
    - Aggregate `TestResultHeader` data by completion status
    - Provide progress tracking data for operational users
    - _Requirements: 6.2_

  - [x] 7.3 Implement billing summary charts for Admin/Accounts roles
    - Create billing trend charts from `TaxInvoice` and `PaymentOrder` data
    - Apply role-based filtering to ensure only Admin/Accounts users see billing charts
    - _Requirements: 6.3_

- [x] 8. Implement Notification Panel
  - [x] 8.1 Implement system notifications from audit logs
    - Query `SiteActivity` table for data export and sensitive access events
    - Transform audit entries into `DashboardNotificationDto` objects
    - _Requirements: 5.1, 5.2, 5.4_

  - [x] 8.2 Implement urgent notifications
    - Query for urgent samples and SLA breach situations
    - Create high-priority notifications for critical situations
    - _Requirements: 5.3_

  - [x] 8.3 Apply role-based notification filtering
    - Filter notifications based on user context and permissions
    - Ensure users only see relevant notifications
    - _Requirements: 5.5_

- [x] 9. Create Dashboard Controller
  - [x] 9.1 Implement `DashboardController` with core endpoints
    - Create controller class with `[Authorize]` attribute
    - Implement `GET /api/dashboard` endpoint
    - Add dependency injection for `IDashboardService` and `LoggedInUserProvider`
    - _Requirements: 7.1_

  - [x] 9.2 Implement JWT claims processing and role detection
    - Use existing `LoggedInUserProvider` to get user context
    - Implement role detection from JWT claims
    - _Requirements: 7.2, 7.4_

  - [x] 9.3 Implement structured JSON response handling
    - Ensure controller returns properly structured JSON with cards, charts, and notifications
    - Add proper HTTP status code handling for different scenarios
    - _Requirements: 7.3, 7.5_




- [x] 10. Add Service Registration and Integration
  - [x] 10.1 Register dashboard service in dependency injection
    - Add `IDashboardService` and `DashboardService` registration to `Program.cs`
    - Follow existing service registration patterns
    - _Requirements: 1.1_

  - [x] 10.2 Add error handling middleware integration
    - Ensure dashboard service integrates with existing `GeneralizedExceptionHandlingMiddleware`
    - Add proper error logging and response formatting
    - _Requirements: 10.1, 10.3, 10.5_

- [x] 11. Final Integration and Testing
  - [x] 11.1 Add comprehensive error handling
    - Implement database connectivity error handling
    - Add validation error responses
    - Ensure no sensitive data is exposed in error messages
    - _Requirements: 10.1, 10.2, 10.3, 10.5_

  - [x] 11.2 Write integration tests for complete dashboard flow
    - Test authentication middleware integration
    - Test role-based response filtering end-to-end
    - Test JSON serialization and HTTP status codes

- [x] 12. Final checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with comprehensive testing ensure robust implementation
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation
- Property tests validate universal correctness properties
- Unit tests validate specific examples and edge cases
- The implementation leverages existing LIMS patterns and infrastructure
- No new database tables are created - all data comes from existing tables
- The dashboard service is read-only and does not modify existing business logic