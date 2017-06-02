﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Intranet.API.Domain.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Intranet.API.Domain.Data;
using Intranet.API.ViewModels;

namespace Intranet.API.Controllers
{
    [Produces("application/json")]
    [Route("/api/v1/profile/{profileId:int}/[controller]")]
    public class ChecklistController : Controller, IEditProfileController<EmployeeToDo>
    {
        private readonly IntranetApiContext _intranetApiContext;

        public ChecklistController(IntranetApiContext intranetApiContext)
        {
            _intranetApiContext = intranetApiContext;
        }

        [AllowAnonymous] // TODO this line is temporary for local testing without authentication, to be removed
        [HttpGet]
        public IActionResult Get(int profileId)
        {
            try
            {
                if (profileId == 0)
                {
                    return BadRequest();
                }

                var employeeTodos = _intranetApiContext.EmployeeToDos.Where(e => e.EmployeeId == profileId).ToList();
                var toDoDescriptions = _intranetApiContext.ToDos.ToList();

                if (employeeTodos.Count == 0 || toDoDescriptions.Count == 0)
                {
                    return NotFound();
                }

                IList<ChecklistViewModel> checklist = new List<ChecklistViewModel>();

                foreach (var toDo in employeeTodos)
                {
                    var taskWithDesc = new ChecklistViewModel()
                    {
                        EmployeeId = profileId,
                        ToDoId = toDo.ToDoId,
                        Done = toDo.Done,
                        Description = toDoDescriptions.Single(t => t.Id == toDo.ToDoId).Description
                    };

                    checklist.Add(taskWithDesc);
                }

                return Ok(checklist);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous] // TODO this line is temporary for local testing without authentication, to be removed
        [Route("{toDoId:int}")]
        [HttpGet]
        public IActionResult Get(int profileId, int toDoId)
        {
            try
            {
                if (profileId == 0 || toDoId == 0)
                {
                    return BadRequest();
                }

                var employeeTodo = _intranetApiContext.EmployeeToDos.SingleOrDefault(e => e.EmployeeId == profileId && e.ToDoId == toDoId);
                var toDo = _intranetApiContext.ToDos.SingleOrDefault(t => t.Id == toDoId);

                if (employeeTodo == null || toDo == null)
                {
                    return NotFound();
                }

                var taskWithDesc = new ChecklistViewModel()
                {
                    EmployeeId = profileId,
                    ToDoId = toDoId,
                    Done = employeeTodo.Done,
                    Description = toDo.Description
                };

                return Ok(taskWithDesc);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous] // TODO this line is temporary for local testing without authentication, to be removed
        [HttpPost]
        public IActionResult Post(int profileId, [FromBody] EmployeeToDo employeeToDo)
        {
            try
            {
                if (profileId != employeeToDo.EmployeeId)
                {
                    ModelState.AddModelError(nameof(EmployeeToDo.EmployeeId), "Incorrect id");
                }

                var checkEmployeeToDoDuplicate = _intranetApiContext.EmployeeToDos.SingleOrDefault(e => e.EmployeeId == profileId && e.ToDoId == employeeToDo.ToDoId);
                var checkThatToDoExist = _intranetApiContext.ToDos.SingleOrDefault(t => t.Id == employeeToDo.ToDoId);

                if (checkEmployeeToDoDuplicate != null || checkThatToDoExist == null)
                {
                    ModelState.AddModelError("", "");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _intranetApiContext.EmployeeToDos.Add(employeeToDo);
                _intranetApiContext.SaveChanges();

                return Ok(ModelState);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous] // TODO this line is temporary for local testing without authentication, to be removed
        [Route("{toDoId:int}")]
        [HttpPut]
        public IActionResult Put(int profileId, int toDoId, [FromBody] EmployeeToDo employeeToDo)
        {
            try
            {
                if (profileId != employeeToDo.EmployeeId)
                {
                    ModelState.AddModelError(nameof(EmployeeToDo.EmployeeId), "Incorrect id");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var checkThatToDoExist = _intranetApiContext.ToDos.SingleOrDefault(t => t.Id == toDoId);
                var objectToChange = _intranetApiContext.EmployeeToDos.SingleOrDefault(e => e.EmployeeId == profileId && e.ToDoId == toDoId);

                if (checkThatToDoExist == null || objectToChange == null)
                {
                    return NotFound(ModelState);
                }

                var checkIfDesiredChangeAlreadyExist = _intranetApiContext.EmployeeToDos.SingleOrDefault(e => e.EmployeeId == profileId && e.ToDoId == toDoId);

                if (checkIfDesiredChangeAlreadyExist != null)
                {
                    return BadRequest(ModelState);
                }

            objectToChange.Done = employeeToDo.Done;
            _intranetApiContext.SaveChanges();

                return Ok(ModelState);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous] // TODO this line is temporary for local testing without authentication, to be removed
        [Route("{toDoId:int}")]
        [HttpDelete]
        public IActionResult Delete(int profileId, int toDoId)
        {
            try
            {
                var employeeToDoToDelete = _intranetApiContext.EmployeeToDos.SingleOrDefault(e => e.EmployeeId == profileId && e.ToDoId == toDoId);

                if (employeeToDoToDelete == null)
                {
                    var error = Json("Checklist task can't be found");
                    return NotFound(error);
                }

                _intranetApiContext.EmployeeToDos.Remove(employeeToDoToDelete);
                _intranetApiContext.SaveChanges();

                return Ok(employeeToDoToDelete);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}