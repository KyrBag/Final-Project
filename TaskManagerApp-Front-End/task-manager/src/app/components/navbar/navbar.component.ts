import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, NgZone, Renderer2, ViewChild, inject } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import {MatMenuModule} from "@angular/material/menu"
import { CommonModule } from '@angular/common';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import {MatDrawer, MatSidenavModule} from '@angular/material/sidenav'
import {MatListModule} from '@angular/material/list';
import { animate, state, style, transition, trigger } from '@angular/animations';


@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    MatToolbarModule, 
    RouterLink, 
    MatMenuModule, 
    MatButtonModule, 
    MatSnackBarModule,
    MatIconModule,
    CommonModule,
    MatSidenavModule,
    MatListModule,
    RouterOutlet,
   
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
  animations: [
    // Animation for drawer open/close
    trigger('drawerState', [
      state('open', style({
        transform: 'translateX(0)',
        visibility: 'visible'
      })),
      state('closed', style({
        transform: 'translateX(-100%)',
        visibility: 'hidden'
      })),
      transition('open <=> closed', [
        animate('0.3s ease-in-out')
      ])
    ])
  ]
})
export class NavbarComponent  implements AfterViewInit {
  authService = inject(AuthService);
  matSnackBar = inject(MatSnackBar)
  router = inject(Router);
  isMenuOpen = false;
 
  @ViewChild('drawer') drawer: MatDrawer;
  drawerOpenState: 'open' | 'closed' = 'closed'; // Initial state of the drawer
  private initialTimeout: any;

  constructor(
    private renderer: Renderer2,
    private zone: NgZone,
    private elementRef: ElementRef
  ) {}

  ngAfterViewInit(): void {
    // Open drawer for 2 seconds after component initializes
    this.initialTimeout = setTimeout(() => {
      this.zone.run(() => {
        this.openDrawer();
        setTimeout(() => this.closeDrawer(), 2000);
      });
    }, 0);

    // Close drawer when clicking outside of it
    this.renderer.listen('document', 'click', (event: Event) => {
      this.handleOutsideClick(event);
    });
  }

  toggleDrawer(): void {
    if (this.drawerOpenState === 'closed') {
      this.openDrawer();
    } else {
      this.closeDrawer();
    }
  }

  openDrawer(): void {
    this.drawer.open();
    this.drawerOpenState = 'open';
  }

  closeDrawer(): void {
    this.drawer.close();
    this.drawerOpenState = 'closed';
  }

  private handleOutsideClick(event: Event): void {
    const target = event.target as HTMLElement;
    if (this.drawer.opened && !this.elementRef.nativeElement.contains(target) && !this.isToggleButton(target)) {
      this.zone.run(() => {
        this.closeDrawer();
      });
    }
  }

  private isToggleButton(target: HTMLElement): boolean {
    return target.classList.contains('hum') || target.closest('.hum') !== null;
  }

  ngOnDestroy(): void {
    clearTimeout(this.initialTimeout); // Clear timeout to prevent memory leaks
  }

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }
 
  isLoggedIn(){
    return this.authService.isLoggedIn();
  }

  logout=()=>{
    this.authService.logout();
    this.matSnackBar.open('Logout success', "Close", {
      duration:5000,
      horizontalPosition: 'center'
    })
    this.router.navigate(['/login']);
  };
}
