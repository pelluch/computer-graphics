close all
eye = [278 273 -800];
target = [278 273 -700];
up = [0 1 0];
fov = 0.68601;
w = (eye-target);
w = w/norm(w);
u = cross(up, w);
u = u/norm(u);
v = cross(w, u);
v = v/norm(v);
near = 0.035;
far = 1500;
tanAngle = tan(fov/2.0);
top = tanAngle*near;
bottom = -top;
right = abs(top);
left = -right;
x=170:10:350;
[X,Y] = meshgrid(x);
Z = 0*X + near - 800;
Z2 = 0*X + far - 800;
for i=1:20:200
    for j=1:20:200
figure(1);
axis([0 500 0 500 -1000 800]);
axis on
axis vis3d
axis equal
axis xy

xlabel('x');
ylabel('y');
zlabel('z');
hold on;
%arrow(origin, target);
%rrow(origin, eye);

plot3(278, 273, -800, 'ro');
plot3(278, 273, -700, 'bo');
arrow(eye, target, 'Length', 5);
arrow(eye, eye+w*200, 'Length', 5);
arrow(eye, eye+v*200, 'Length', 5);
arrow(eye, eye+u*200, 'Length', 5);

[x, y, z] = sphere;

surf(120*x+370, 120*y+120, 120*z+370);
surf(100*x+130, 100*y+100, 100*z+130);

u_s = (i+0.5)*(right-left)/200 + left;
v_s = (j+0.5)*(top-bottom)/200 + bottom;
w_s = -near;

sij = eye + u_s*u +v_s*v +w_s*w;
dir = (sij-eye);
dir = dir/norm(dir);
arrow(eye, eye+dir*500, 'Length', 5);

%pause
%surf(X,Y,Z);
surf(X,Y,Z2);
u_s
v_s

hold off;
    end
end
%arrow(origin, up);