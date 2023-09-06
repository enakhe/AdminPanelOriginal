<td class="dropdown">
                                                        <a class="text-dark p" href="#" data-toggle="dropdown" aria-expanded="false">
                                                            <i class="fa-solid fa-ellipsis-vertical"></i>
                                                        </a>
                                                        <ul class="dropdown-menu dropdown-menu-md dropdown-menu-center">
                                                            <li class="dropdown-item">
                                                                <a asp-area="Admin" class="text-primary" asp-page="/Users/Edit" asp-route-id="@user.Id">
                                                                    <i class="fa-solid text-primary fa-edit"></i> Edit
                                                                </a>
                                                            </li>
                                                            <li class="dropdown-item">
                                                                <a asp-area="Admin" class="text-danger" asp-page="/Users/Delete" asp-route-id="@user.Id">
                                                                    <i class="fa-solid text-danger fa-trash"></i> Delete
                                                                </a>
                                                            </li>
                                                            <li class="dropdown-item">
                                                                <a asp-area="Admin" class="text-dark" asp-page="/Users/Role" asp-route-id="@user.Id">
                                                                    <i class="fa-solid fa-layer-group"></i> Manage role
                                                                </a>
                                                            </li>
                                                        </ul>
                                                    </td>