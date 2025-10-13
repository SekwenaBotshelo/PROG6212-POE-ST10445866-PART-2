# PROG6212-POE
Student Name: Botshelo Koketso Sekwena

Student Number: ST10445866

GitHub Repository: https://github.com/SekwenaBotshelo/PROG6212-POE-ST10445866-PART-2.git 

Project Description

The Claims Management System is a comprehensive web application designed to streamline the process of managing academic claims within an educational institution. The system provides distinct interfaces for Lecturers, Coordinators, and Managers to handle claim submission, verification, approval, and reporting.

Key Features

For Lecturers:

- Submit new claims with detailed information
- Upload supporting documents (PDF, DOCX, XLSX)
- Track claim status in real-time
- View personal claim history

For Coordinators:

- Verify submitted claims for accuracy
- Approve or reject claims with comments
- Monitor pending verification queue
- View detailed claim information

For Managers:

- Approve verified claims
- Generate comprehensive reports
- View analytics and summary statistics
- Monitor overall claim processing

Technical Design

Architecture Overview

The application follows a Model-View-Controller (MVC) architecture pattern with clear separation of concerns:

PROG6212-POE/

├── Controllers/

│   ├── HomeController.cs          # Main navigation controller

│   ├── LecturerController.cs      # Lecturer-specific functionality

│   ├── CoordinatorController.cs   # Coordinator verification features

│   └── ManagerController.cs       # Manager approval and reporting

├── Models/

│   ├── Claim.cs                   # Main claim data model

│   ├── ClaimStatus.cs            # Status enumeration

│   └── ErrorViewModel.cs         # Error handling model

├── Views/

│   ├── Home/                     # Public pages

│   │   ├── Index.cshtml

│   │   ├── About.cshtml

│   │   ├── Contact.cshtml

│   │   └── Privacy.cshtml

│   ├── Lecturer/                 # Lecturer interface

│   │   ├── Dashboard.cshtml

│   │   ├── SubmitClaim.cshtml

│   │   ├── TrackStatus.cshtml

│   │   └── UploadDocument.cshtml

│   ├── Coordinator/              # Coordinator interface

│   │   ├── Dashboard.cshtml

│   │   ├── VerifyClaims.cshtml

│   │   ├── VerifyClaimDetails.cshtml

│   │   └── Reports.cshtml

│   └── Manager/                  # Manager interface

│       ├── Dashboard.cshtml

│       ├── ApproveClaims.cshtml

│       ├── ApproveClaimDetails.cshtml

│       └── Reports.cshtml

├── Tests/                        # xUnit test project

│   ├── CoordinatorControllerTests.cs

│   ├── LecturerControllerTests.cs

│   └── ManagerControllerTests.cs

└── Program.cs                    # Application entry point


Bibliography
