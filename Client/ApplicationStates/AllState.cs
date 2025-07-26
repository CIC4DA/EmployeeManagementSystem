namespace Client.ApplicationStates
{
    // This will be used to control general department state
    // we will use dependency injection
    public class AllState
    {
        // scope action
        public Action? Action { get; set; }

        // User
        public bool ShowUser { get; set; }

        public void UserClicked()
        {
            ResetAllDepartments();
            ShowUser = true;
            // The component subscribe to this state
            Action?.Invoke();
        }


        // Employee
        public bool ShowEmployee { get; set; } = true;

        public void EmployeeClicked()
        {
            ResetAllDepartments();
            ShowEmployee = true;
            // The component subscribe to this state
            Action?.Invoke();
        }


        // GeneralDepartment
        public bool ShowGeneralDepartment { get; set; }

        public void GeneralDepartmentClicked()
        {
            ResetAllDepartments();
            ShowGeneralDepartment = true;
            // The component subscribe to this state
            Action?.Invoke();
        }

        // Department
        public bool ShowDepartment { get; set; }

        public void DepartmentClicked()
        {
            ResetAllDepartments();
            ShowDepartment = true;
            // The component subscribe to this state
            Action?.Invoke();
        }

        // Branch
        public bool ShowBranch { get; set; }

        public void BranchClicked()
        {
            ResetAllDepartments();
            ShowBranch = true;
            // The component subscribe to this state
            Action?.Invoke();
        }


        // Country
        public bool ShowCountry { get; set; }

        public void CountryClicked()
        {
            ResetAllDepartments();
            ShowCountry = true;
            // The component subscribe to this state
            Action?.Invoke();
        }

        // City
        public bool ShowCity { get; set; }

        public void CityClicked()
        {
            ResetAllDepartments();
            ShowCity = true;
            // The component subscribe to this state
            Action?.Invoke();
        }

        // Town
        public bool ShowTown { get; set; }

        public void TownClicked()
        {
            ResetAllDepartments();
            ShowTown = true;
            // The component subscribe to this state
            Action?.Invoke();
        }

        // Health
        public bool ShowHealth { get; set; }

        public void HealthClicked()
        {
            ResetAllDepartments();
            ShowHealth = true;
            // The component subscribe to this state
            Action?.Invoke();
        }

        // Overtime
        public bool ShowOvertime { get; set; }
        public bool ShowOvertimeType { get; set; }

        public void OvertimeClicked()
        {
            ResetAllDepartments();
            ShowOvertime = true;
            // The component subscribe to this state
            Action?.Invoke();
        }
        public void OvertimeTypeClicked()
        {
            ResetAllDepartments();
            ShowOvertimeType = true;
            // The component subscribe to this state
            Action?.Invoke();
        }

        // Sanction
        public bool ShowSanction { get; set; }
        public bool ShowSanctionType { get; set; }

        public void SanctionClicked()
        {
            ResetAllDepartments();
            ShowSanction = true;
            // The component subscribe to this state
            Action?.Invoke();
        }
        public void SanctionTypeClicked()
        {
            ResetAllDepartments();
            ShowSanctionType = true;
            // The component subscribe to this state
            Action?.Invoke();
        }

        // Vacation
        public bool ShowVacation { get; set; }
        public bool ShowVacationType { get; set; }

        public void VacationClicked()
        {
            ResetAllDepartments();
            ShowVacation = true;
            // The component subscribe to this state
            Action?.Invoke();
        }
        public void VacationTypeClicked()
        {
            ResetAllDepartments();
            ShowVacationType = true;
            // The component subscribe to this state
            Action?.Invoke();
        }

        // Ressting all 
        private void ResetAllDepartments()
        {
            ShowGeneralDepartment = false;
            ShowDepartment = false;
            ShowBranch = false;
            ShowCountry = false;
            ShowCity = false;
            ShowTown = false;
            ShowUser = false;
            ShowEmployee= false;
            ShowHealth = false;
            ShowOvertime = false;
            ShowOvertimeType = false;
            ShowSanction = false;
            ShowSanctionType = false;
            ShowVacation = false;
            ShowVacationType = false;
        }
    }
}
