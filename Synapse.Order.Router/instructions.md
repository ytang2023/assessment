Please follow the instructions below and complete the implementation end-to-end.

Before you start, review the existing solution structure and follow the current project patterns where possible. If anything is unclear, ask now. Otherwise, proceed independently and iterate until the solution builds and all tests pass.

Tasks:

1. Product CSV Data Provider
- Add `products.csv` as an embedded resource.
- Load the embedded resource at runtime.
- Use CsvHelper to parse the CSV rows.
- Create a strongly typed model class for the product rows.
- Add one test in `EnsureProductDataProvider.cs` to verify the provider behavior.

2. Supplier CSV Data Provider
- Add `supplier.csv` as an embedded resource.
- Load and parse it using the same pattern as the products CSV.
- Create a strongly typed model class for the supplier rows.
- Add one test in `EnsureSupplierDataProvider.cs` to verify the provider behavior.

3. Router Engine
- Complete the routing logic in `RouterEngine.cs`.
- Update `RouterService.cs` so it uses `RouterEngine`.
- Add one theory-based test in `EnsureRouter.cs`.
- The theory should verify the different routing cases provided in `sample_orders.json`.

4. Integration Through IOC
- Wire everything together in `RouterService.cs`.
- Add one test that uses the IOC container to verify the full flow works correctly.
- The test should confirm that the registered dependencies resolve correctly and the router service behaves as expected.

5. OpenAPI Endpoint
- Use the existing OpenAPI package to create an endpoint based on the task assignment.
- Add a request example in a separate class file so the endpoint is easy to test.
- Make sure Swagger runs locally at:

  `http://localhost:7080/api/swagger/ui`

6. Build and Test
- Run the full solution build.
- Run all tests.
- Fix any issues you find.
- Continue iterating until the solution builds successfully and all tests pass.

7. Containerization
- Add the required Dockerfile and container-related configuration.
- Make sure the service can run in a container.
- Configure the containerized service so Swagger is available at:

  `http://localhost:7071/api/swagger/ui`

  This avoids conflict with the local non-containerized service.

Code Standards:

- Do not use `try/catch` except at the service or controller/API boundary level.
- Avoid deeply nested code. Nesting should not exceed 5 levels.
- Each model must be placed in its own file.
- Follow the existing naming conventions and folder structure.
- Keep the implementation clean, testable, and easy to extend.

Please complete the work, run the tests, fix failures, and only stop when everything builds and passes.