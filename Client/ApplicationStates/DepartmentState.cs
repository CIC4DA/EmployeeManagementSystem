namespace Client.ApplicationStates
{
    // This will be used to control general department state
    // we will use dependency injection
    public class DepartmentState
    {
        public Action? GeneralDepartmentAction { get; set; }
        public bool ShowGeneralDepartment { get; set; }

        public void GeneralDepartmentClicked()
        {
            ResetAllDepartments();
            ShowGeneralDepartment = true;
            // The component subscribe to this state
            GeneralDepartmentAction?.Invoke();
        }

        private void ResetAllDepartments()
        {
            ShowGeneralDepartment = false;
        }
    }
}
